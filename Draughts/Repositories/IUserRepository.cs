using Draughts.Domain.UserContext.Models;

namespace Draughts.Repositories {
    public interface IUserRepository : IRepository<User, UserId> {
    }
}
