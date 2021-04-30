using Draughts.Common.Utilities;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;
using System;

namespace Draughts.Domain.GameAggregate.Services {
    public class PlayGameDomainService : IPlayGameDomainService {
        private readonly IClock _clock;

        public PlayGameDomainService(IClock clock) {
            _clock = clock;
        }

        public void DoMove(Game game, GameState gameState, UserId currentUserId, SquareId from, SquareId to) {
            game.ValidateCanDoMove(currentUserId);

            var moveResult = gameState.AddMove(from, to, game.Turn!.Player.Color, game.Settings);

            switch (moveResult) {
                case GameState.MoveResult.NextTurn:
                    game.NextTurn(currentUserId, _clock.UtcNow());
                    break;
                case GameState.MoveResult.MoreCapturesAvailable:
                    break; // No need to do anything, the user will do the next move.
                case GameState.MoveResult.GameOver:
                    game.WinGame(currentUserId, _clock.UtcNow());
                    break;
                default:
                    throw new InvalidOperationException("Unknown MoveResult");
            }
        }
    }
}
