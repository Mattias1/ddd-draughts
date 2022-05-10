using Draughts.Common.Utilities;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.Events;

public sealed class EventQueue {
    private readonly IClock _clock;
    private readonly IReadOnlyList<IDomainEventHandler> _eventHandlers;
    private readonly Queue<DomainEvent> _queue;
    private readonly Queue<DomainEvent> _failedEventsQueue;

    public EventQueue(IClock clock, IReadOnlyList<IDomainEventHandler> eventHandlers) {
        _clock = clock;
        _eventHandlers = eventHandlers;
        _failedEventsQueue = new Queue<DomainEvent>();
        _queue = new Queue<DomainEvent>();
    }

    public void Enqueue(IEnumerable<DomainEvent> evts) => evts.ForEach(evt => Enqueue(evt));
    public void Enqueue(DomainEvent evt) => _queue.Enqueue(evt);

    public bool DispatchNext() {
        bool handled = DispatchEvent(_queue.Dequeue());

        RequeueFailedEvents();

        return handled;
    }

    public bool DispatchAll() {
        bool allAreHandled = true;
        while (_queue.TryDequeue(out var evt)) {
            allAreHandled &= DispatchEvent(evt);
        }

        RequeueFailedEvents();

        return allAreHandled;
    }

    private bool DispatchEvent(DomainEvent evt) {
        try {
            return _eventHandlers
                .Where(h => h.CanHandle(evt))
                .ForEach(h => h.Handle(evt))
                .Any();
        }
        catch (Exception) {
            _failedEventsQueue.Enqueue(evt);
            throw;
            // return false;
        }
    }

    private void RequeueFailedEvents() {
        foreach (var evt in _failedEventsQueue) {
            evt.RegisterFailedAttempt(_clock.UtcNow());
            _queue.Enqueue(evt);
        }
    }
}
