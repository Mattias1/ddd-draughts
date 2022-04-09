using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;
using static Draughts.Domain.GameContext.Models.Voting;

namespace Draughts.Domain.GameContext.Services;

public sealed class PlayGameDomainService {
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

    public void VoteForDraw(Game game, Voting voting, UserId currentUserId) {
        game.ValidateCanVote(currentUserId);
        voting.VoteFor(currentUserId, VotingSubject.Draw, _clock.UtcNow());

        if (voting.AreAllInFavor(VotingSubject.Draw)) {
            game.Draw(_clock.UtcNow());
        }
    }
}
