using DalSoft.Hosting.BackgroundQueue;
using Draughts.Common.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Draughts.Common.Events;

public sealed class EventDispatcher {
    private readonly BackgroundQueue _backgroundQueue;
    private readonly List<IDomainEventHandler> _eventHandlers;
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(BackgroundQueue backgroundQueue, ILogger<EventDispatcher> logger) {
        _backgroundQueue = backgroundQueue;
        _eventHandlers = new List<IDomainEventHandler>();
        _logger = logger;
    }

    public void Register(IEnumerable<IDomainEventHandler> eventHandlers) => _eventHandlers.AddRange(eventHandlers);

    public bool DispatchAll(IReadOnlyList<DomainEvent> events) => events.All(DispatchEvent);

    private bool DispatchEvent(DomainEvent evt) {
        try {
            bool success = false;
            foreach (var handler in _eventHandlers) {
                if (handler.CanHandle(evt)) {
                    _backgroundQueue.Enqueue(cancellationToken => {
                        handler.Handle(evt);
                        return Task.CompletedTask;
                    });
                    success = true;
                }
            }
            if (!success) {
                _logger.LogError($"No event handler found for {evt.Type}.");
            }
            return success;
        }
        catch (Exception e) {
            _logger.LogError("Uncaught exception", e);
            return false;
        }
    }
}
