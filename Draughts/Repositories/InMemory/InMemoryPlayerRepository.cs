using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.UserAggregate.Models.Rank;

namespace Draughts.Repositories.InMemory {
    public class InMemoryPlayerRepository : InMemoryRepository<Player>, IPlayerRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryPlayerRepository(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        protected override IList<Player> GetBaseQuery() {
            return GameDatabase.PlayersTable.Select(p => new Player(
                new PlayerId(p.Id),
                new UserId(p.UserId),
                new Username(p.Username),
                Ranks.All.Single(r => r.Name == p.Rank),
                p.ColorIsWhite ? Color.White : Color.Black,
                p.CreatedAt
            )).ToList();
        }

        public void Save(Player entity, GameId gameId) => Save(entity);
        public override void Save(Player entity) {
            var player = new InMemoryPlayer {
                Id = entity.Id,
                UserId = entity.UserId,
                Username = entity.Username,
                Rank = entity.Rank.Name,
                ColorIsWhite = entity.Color == Color.White,
                CreatedAt = entity.CreatedAt
            };

            _unitOfWork.Store(player, GameDatabase.TempPlayersTable);
        }
    }
}
