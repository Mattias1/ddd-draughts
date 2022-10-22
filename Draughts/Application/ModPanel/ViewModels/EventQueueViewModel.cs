using Draughts.Common.Events;
using Draughts.Repositories.Misc;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.ModPanel.ViewModels;

public sealed class EventQueueViewModel {
    public IReadOnlyList<EventViewModel> AuthEvents => AuthEventsPagination.Results;
    public IReadOnlyList<EventViewModel> UserEvents => UserEventsPagination.Results;
    public IReadOnlyList<EventViewModel> GameEvents => GameEventsPagination.Results;

    public Pagination<EventViewModel> AuthEventsPagination { get; }
    public Pagination<EventViewModel> UserEventsPagination { get; }
    public Pagination<EventViewModel> GameEventsPagination { get; }
    public Pagination<EventViewModel> LargestPagination { get; }

    public EventQueueViewModel(Pagination<DomainEvent> authEvents, Pagination<DomainEvent> userEvents,
            Pagination<DomainEvent> gameEvents) {
        AuthEventsPagination = authEvents.Map(e => new EventViewModel(e));
        UserEventsPagination = userEvents.Map(e => new EventViewModel(e));
        GameEventsPagination = gameEvents.Map(e => new EventViewModel(e));

        var allPaginations = new[] { AuthEventsPagination, UserEventsPagination, GameEventsPagination };
        LargestPagination = allPaginations.MaxBy(p => p.Count) ?? AuthEventsPagination;
    }
}

public sealed class EventViewModel {
    public DomainEventId Id { get; }
    public string Type { get; }
    public ZonedDateTime CreatedAt { get; }
    public ZonedDateTime? HandledAt { get; }

    public EventViewModel(DomainEvent evt) {
        Id = evt.Id;
        Type = evt.Type;
        CreatedAt = evt.CreatedAt;
        HandledAt = evt.HandledAt;
    }
}
