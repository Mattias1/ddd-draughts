using Draughts.Domain.UserAggregate.Models;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Controllers.Shared.ViewModels {
    public class UserlistViewModel {
        public IReadOnlyList<UserViewModel> Users { get; set; }

        public UserlistViewModel(IReadOnlyList<User> users) {
            Users = users.Select(u => new UserViewModel(u)).ToList().AsReadOnly();
        }
    }
}
