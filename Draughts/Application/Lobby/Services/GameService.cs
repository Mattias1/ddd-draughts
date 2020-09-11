using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Databases;

namespace Draughts.Application.Lobby.Services {
    // Note: This name is way to generic. In the future I'll put everything in here. So I'll rename it then :)
    public class GameService : IGameService {
        private readonly IGameFactory _gameFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public GameService(IGameFactory gameFactory, IUnitOfWork unitOfWork, IUserRepository userRepository) {
            _gameFactory = gameFactory;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public void CreateGame(UserId userId, GameSettings gameSettings, Color joinColor) {
            var user = _userRepository.FindById(userId);

            _unitOfWork.WithTransaction(TransactionDomain.Game, tran => {
                _gameFactory.CreateGame(gameSettings, user, joinColor);

                tran.Commit();
            });
        }
    }
}
