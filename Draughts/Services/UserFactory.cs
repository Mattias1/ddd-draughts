using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;

namespace Draughts.Services {
    public class UserFactory : IUserFactory {
        private readonly IEventQueue _eventQueue;
        private readonly IUserRepository _userRepository;

        public UserFactory(IEventQueue eventQueue, IUserRepository userRepository) {
            _eventQueue = eventQueue;
            _userRepository = userRepository;
        }

        public User CreateUser(AuthUserId authUserId, UserId userId, Username username) {
            var user = new User(userId, authUserId, username, Rating.StartRating, Rank.Ranks.Private, 0);
            _userRepository.Save(user);

            _eventQueue.Raise(new UserCreated(user));

            return user;
        }
    }
}
