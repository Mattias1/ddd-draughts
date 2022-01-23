using Draughts.Repositories;

namespace Draughts.Application.Shared.ViewModels;

public interface IPaginationViewModel<T> {
    Pagination<T> Pagination { get; }
}
