using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Controllers.Shared.ViewModels {
    public class PlayerViewModel {
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
}
