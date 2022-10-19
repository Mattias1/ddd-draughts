using DalSoft.Hosting.BackgroundQueue;
using Draughts.Common.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draughts.Common.Events;

public sealed class EventDispatcher {
    private readonly BackgroundQueue _backgroundQueue;
    private readonly List<IDomainEventHandler> _eventHandlers;
    private readonly ILogger<EventDispatcher> _logger;

    private static readonly object _lock = new object();
    private static readonly HashSet<long> _lockedEvents = new HashSet<long>();

    public EventDispatcher(BackgroundQueue backgroundQueue, ILogger<EventDispatcher> logger) {
        _backgroundQueue = backgroundQueue;
        _eventHandlers = new List<IDomainEventHandler>();
        _logger = logger;
    }

    public void Register(IEnumerable<IDomainEventHandler> eventHandlers) => _eventHandlers.AddRange(eventHandlers);

    public void DispatchAll(IReadOnlyList<DomainEvent> events) => events.ForEach(DispatchEvent);

    private void DispatchEvent(DomainEvent evt) {
        try {
            bool foundHandler = false;
            foreach (var handler in _eventHandlers) {
                if (handler.CanHandle(evt)) {
                    foundHandler = true;
                    _backgroundQueue.Enqueue(cancellationToken => {
                        bool lockedEvent = false;
                        try {
                            lock(_lock) {
                                if (_lockedEvents.Contains(evt.Id.Value)) {
                                    _logger.LogWarning($"Trying to dispatch event {evt.Id}, but it's locked.");
                                    return Task.CompletedTask;
                                }
                                lockedEvent = true;
                                _lockedEvents.Add(evt.Id.Value);
                            }

                            handler.Handle(evt);
                            return Task.CompletedTask;
                        }
                        finally {
                            if (lockedEvent) {
                                _lockedEvents.Remove(evt.Id.Value);
                            }
                        }
                    });
                }
            }
            if (!foundHandler) {
                _logger.LogError($"No event handler found for {evt.Type}.");
            }
        }
        catch (Exception e) {
            _logger.LogError("Uncaught exception", e);
        }
    }
}
