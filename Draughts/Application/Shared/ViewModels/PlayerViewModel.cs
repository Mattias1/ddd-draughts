using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;

namespace Draughts.Application.Shared.ViewModels;

public sealed class PlayerViewModel {
    public PlayerId Id { get; }
    public UserId UserId { get; }
    public Username Username { get; }
    public Rank Rank { get; }
    public Color Color { get; }

    public PlayerViewModel(Player player) {
        Id = player.Id;
        UserId = player.UserId;
        Username = player.Username;
        Rank = player.Rank;
        Color = player.Color;
    }
}
