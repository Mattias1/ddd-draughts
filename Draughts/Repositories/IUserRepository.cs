using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Repositories {
    public interface IUserRepository : IRepository<User> {
        User FindById(UserId id);
        User? FindByIdOrNull(UserId id);
    }
}
