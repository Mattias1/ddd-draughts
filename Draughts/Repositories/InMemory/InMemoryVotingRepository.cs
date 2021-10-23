using Draughts.Common.OoConcepts;
using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryVotingRepository : IVotingRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryVotingRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public long Count() => GetBaseQuery().Count();
        public long Count(Specification<Voting> spec) => GetBaseQuery().Count(spec.IsSatisfiedBy);

        public Voting FindById(GameId id) => Find(new EntityIdSpecification<Voting, GameId>(id));
        public Voting? FindByIdOrNull(GameId id) => FindOrNull(new EntityIdSpecification<Voting, GameId>(id));

        public Voting Find(Specification<Voting> spec) => List(spec).Single();
        public Voting? FindOrNull(Specification<Voting> spec) => List(spec).SingleOrDefault();

        public IReadOnlyList<Voting> List() => GetBaseQuery().ToList().AsReadOnly();
        public IReadOnlyList<Voting> List<GameId>(Sort<Voting, GameId> sort) => GetBaseQuery().Sort(sort).ToList().AsReadOnly();
        public IReadOnlyList<Voting> List(Specification<Voting> spec) => GetBaseQuery().Where(spec.IsSatisfiedBy).ToList().AsReadOnly();
        public IReadOnlyList<Voting> List<GameId>(Specification<Voting> spec, Sort<Voting, GameId> sort)
            => GetBaseQuery().Where(spec.IsSatisfiedBy).Sort(sort).ToList().AsReadOnly();

        public void Save(Voting entity) {
            var votes = DbVote.FromDomainModel(entity);
            votes.ForEach(v => _unitOfWork.Store<DbVote>(v, tran => GameDatabase.Temp(tran).VotesTable));
        }

        protected IList<Voting> GetBaseQuery() {
            return DbVote.ToDomainModels(GameDatabase.Get.VotesTable).ToList();
        }
    }
}
