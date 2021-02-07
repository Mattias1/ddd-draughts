using Draughts.Application.Shared.Services;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using NodaTime;

namespace Draughts.Application.Auth.Services {
    public class UserFactory : IUserFactory {
        private readonly IClock _clock;
        private readonly IEventFactory _eventFactory;
        private readonly IUserRepository _userRepository;

        public UserFactory(IClock clock, IEventFactory eventFactory, IUserRepository userRepository) {
            _clock = clock;
            _eventFactory = eventFactory;
            _userRepository = userRepository;
        }

        public User CreateUser(AuthUserId authUserId, UserId userId, Username username) {
            var user = new User(userId, authUserId, username, Rating.StartRating, Rank.Ranks.Private, 0, _clock.UtcNow());
            _userRepository.Save(user);

            _eventFactory.RaiseUserCreated(user);

            return user;
        }
    }
}
