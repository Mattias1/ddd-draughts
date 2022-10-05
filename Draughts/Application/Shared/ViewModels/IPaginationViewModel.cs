using Draughts.Repositories;
using Draughts.Repositories.Misc;

namespace Draughts.Application.Shared.ViewModels;

public interface IPaginationViewModel<T> {
    Pagination<T> Pagination { get; }
}
