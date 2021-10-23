using Draughts.Common.OoConcepts;
using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.Database {
    public class DbVotingRepository : IVotingRepository {
        private readonly IUnitOfWork _unitOfWork;

        private string TableName => "vote";

        public DbVotingRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public long Count() {
            return GetBaseQuery().Select().CountAll().From(TableName).SingleLong();
        }
        public long Count(Specification<Voting> spec) {
            return GetBaseQuery().Select().CountAll().From(TableName).Specifically(spec).SingleLong();
        }

        public Voting FindById(GameId id) => Find(new EntityIdSpecification<Voting, GameId>(id, "game_id"));
        public Voting? FindByIdOrNull(GameId id) => FindOrNull(new EntityIdSpecification<Voting, GameId>(id, "game_id"));

        public Voting Find(Specification<Voting> spec) => List(spec).Single();
        public Voting? FindOrNull(Specification<Voting> spec) => List(spec).SingleOrDefault();

        public IReadOnlyList<Voting> List() {
            return Parse(GetBaseSelectQuery().List<DbVote>());
        }
        public IReadOnlyList<Voting> List(Specification<Voting> spec) {
            return Parse(GetBaseSelectQuery().Specifically(spec).List<DbVote>());
        }
        public IReadOnlyList<Voting> List<TKey>(Sort<Voting, TKey> sort) {
            return Parse(GetBaseSelectQuery().ApplySort(sort).List<DbVote>());
        }
        public IReadOnlyList<Voting> List<TKey>(Specification<Voting> spec, Sort<Voting, TKey> sort) {
            return Parse(GetBaseSelectQuery().Specifically(spec).ApplySort(sort).List<DbVote>());
        }

        public void Save(Voting entity) {
            // We (currently) can't modify or delete votes, so inserting is all that's necessary here.
            var voteObjs = DbVote.FromDomainModel(entity);
            var existingVotes = GetBaseSelectQuery().Where("game_id").Is(entity.Id).List<DbVote>();
            var newVotes = voteObjs.Except(existingVotes);
            GetBaseQuery().InsertInto(TableName).InsertFrom(newVotes).Execute();
        }

        private IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.Game);
        private IQueryBuilder GetBaseSelectQuery() => GetBaseQuery().SelectAllFrom(TableName);

        private IReadOnlyList<Voting> Parse(IReadOnlyList<DbVote> vs) {
            if (vs.Count == 0) {
                return new List<Voting>().AsReadOnly();
            }

            return DbVote.ToDomainModels(vs);
        }
    }
}
