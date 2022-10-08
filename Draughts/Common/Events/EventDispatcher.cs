using Draughts.Common.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.Events;

public sealed class EventDispatcher {
    private readonly List<IDomainEventHandler> _eventHandlers;
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(ILogger<EventDispatcher> logger) {
        _eventHandlers = new List<IDomainEventHandler>();
        _logger = logger;
    }

    public void Register(IEnumerable<IDomainEventHandler> eventHandlers) => _eventHandlers.AddRange(eventHandlers);

    public bool DispatchAll(IReadOnlyList<DomainEvent> events) => events.All(DispatchEvent);

    private bool DispatchEvent(DomainEvent evt) {
        try {
            bool success = _eventHandlers
                .Where(h => h.CanHandle(evt))
                .ForEach(h => h.Handle(evt))
                .Any();
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
