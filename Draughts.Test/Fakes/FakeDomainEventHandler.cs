using Draughts.Common.Events;
using System.Collections.Generic;

namespace Draughts.Test.Fakes;

public class FakeDomainEventHandler : IDomainEventHandler {
    public List<DomainEvent> HandledEvents { get; } = new List<DomainEvent>();

    public bool CanHandle(DomainEvent evt) => true;

    public void Handle(DomainEvent evt) => HandledEvents.Add(evt);
}
