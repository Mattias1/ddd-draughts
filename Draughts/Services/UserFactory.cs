using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;

namespace Draughts.Services {
    public class UserFactory : IUserFactory {
        private readonly IEventFactory _eventFactory;
        private readonly IUserRepository _userRepository;

        public UserFactory(IEventFactory eventFactory, IUserRepository userRepository) {
            _eventFactory = eventFactory;
            _userRepository = userRepository;
        }

        public User CreateUser(AuthUserId authUserId, UserId userId, Username username) {
            var user = new User(userId, authUserId, username, Rating.StartRating, Rank.Ranks.Private, 0);
            _userRepository.Save(user);

            _eventFactory.RaiseUserCreated(user);

            return user;
        }
    }
}
