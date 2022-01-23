using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;

namespace Draughts.Repositories.Database;

public class DbGameStateRepository : DbRepository<GameState, GameId, DbGameState>, IGameStateRepository {
    private readonly IUnitOfWork _unitOfWork;

    public DbGameStateRepository(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }

    protected override string TableName => "gamestate";
    private const string MoveTableName = "move";
    private const string GameTableName = "game"; // Read only; this belongs to a different aggregate.
    protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.Game);

    protected override GameState Parse(DbGameState gs) {
        var settings = GetBaseQuery().SelectAllFrom(GameTableName)
            .Where("id").Is(gs.Id)
            .Single<DbGame>()
            .GetGameSettings();
        var moves = GetBaseQuery().SelectAllFrom(MoveTableName)
            .Where("game_id").Is(gs.Id)
            .OrderByAsc("index")
            .List<DbMove>();
        return gs.ToDomainModel(settings, moves);
    }

    public override void Save(GameState entity) {
        var obj = DbGameState.FromDomainModel(entity);
        if (FindByIdOrNull(entity.Id) is null) {
            GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
        }
        else {
            // There's no reason to update a game state entity, nothing can change
        }

        int maxDbIndex = GetBaseQuery().Select().Max("index")
            .From(MoveTableName)
            .Where("game_id").Is(entity.Id)
            .SingleOrDefaultValue<short>() ?? -1;
        if (entity.Moves.Count > maxDbIndex + 1) {
            var newMoves = DbMove.ArrayFromDomainModels(entity, maxDbIndex);
            GetBaseQuery().InsertInto(MoveTableName).InsertFrom(newMoves).Execute();
        }
    }
}
