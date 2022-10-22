using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Misc;
using System.Collections.Generic;

namespace Draughts.Application.Shared.ViewModels;

public sealed class UserlistViewModel : IPaginationViewModel<UserViewModel> {
    public IReadOnlyList<UserViewModel> Users => Pagination.Results;
    public Pagination<UserViewModel> Pagination { get; }

    public UserlistViewModel(Pagination<User> pagination) {
        Pagination = pagination.Map(u => new UserViewModel(u));
    }
}
