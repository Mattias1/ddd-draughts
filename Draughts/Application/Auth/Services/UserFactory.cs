using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.Auth.Services {
    public class UserFactory : IUserFactory {
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UserFactory(IClock clock, IUnitOfWork unitOfWork, IUserRepository userRepository) {
            _clock = clock;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public User CreateUser(UserId userId, Username username) {
            var user = new User(userId, username, Rating.StartRating, Rank.Ranks.Private, 0, _clock.UtcNow());
            _userRepository.Save(user);

            _unitOfWork.Raise(UserCreated.Factory(user));

            return user;
        }
    }
}
