using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Services;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.PlayGame.Services;

public sealed class PlayGameService {
    private readonly IClock _clock;
    private readonly GameRepository _gameRepository;
    private readonly GameStateRepository _gameStateRepository;
    private readonly PlayGameDomainService _playGameDomainService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly VotingRepository _votingRepository;

    public PlayGameService(IClock clock, GameRepository gameRepository, GameStateRepository gameStateRepository,
            PlayGameDomainService playGameDomainService, IUnitOfWork unitOfWork, VotingRepository votingRepository) {
        _clock = clock;
        _gameRepository = gameRepository;
        _gameStateRepository = gameStateRepository;
        _playGameDomainService = playGameDomainService;
        _unitOfWork = unitOfWork;
        _votingRepository = votingRepository;
    }

    public (Game game, GameState gameState) DoMove(UserId currentUserId, GameId gameId, SquareId from, SquareId to) {
        return _unitOfWork.WithGameTransaction(tran => {
            var (game, gameState) = FindGameAndState(gameId);
            _playGameDomainService.DoMove(game, gameState, currentUserId, from, to);

            _gameRepository.Save(game);
            _gameStateRepository.Save(gameState);

            return (game, gameState);
        });
    }

    public void VoteForDraw(UserId currentUserId, GameId gameId) {
        _unitOfWork.WithGameTransaction(tran => {
            var game = FindGame(gameId);
            var voting = _votingRepository.FindByIdOrNull(gameId) ?? Voting.StartNew(gameId);

            _playGameDomainService.VoteForDraw(game, voting, currentUserId);

            _votingRepository.Save(voting);
            _gameRepository.Save(game);
        });
    }

    public void Resign(UserId currentUserId, GameId gameId) {
        _unitOfWork.WithGameTransaction(tran => {
            var game = FindGame(gameId);
            game.ResignGame(currentUserId, _clock.UtcNow());
            _gameRepository.Save(game);
        });
    }

    public (Game game, GameState gameState) FindGameAndState(GameId gameId) {
        var game = _gameRepository.FindByIdOrNull(gameId);
        var gameState = _gameStateRepository.FindByIdOrNull(gameId);
        if (game is null || gameState is null) {
            throw new ManualValidationException("Game not found.");
        }
        return (game, gameState);
    }

    public Game FindGame(GameId gameId) {
        var game = _gameRepository.FindByIdOrNull(gameId);
        if (game is null) {
            throw new ManualValidationException("Game not found.");
        }
        return game;
    }
}
