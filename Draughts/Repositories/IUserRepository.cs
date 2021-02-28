using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Repositories {
    public interface IUserRepository : IRepository<User, UserId> {
    }
}
