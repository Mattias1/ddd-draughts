using Draughts.Domain.GameContext.Models;

namespace Draughts.Repositories {
    public interface IVotingRepository : IRepositoryWithoutPagination<Voting, GameId> {
    }
}
