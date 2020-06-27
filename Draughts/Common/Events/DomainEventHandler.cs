using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.Events {
    public abstract class DomainEventHandler : IDomainEventHandler {
        protected IReadOnlyList<string> RecognizedTypes { get; }

        public DomainEventHandler(params string[] recognizedTypes) {
            if (recognizedTypes.Length == 0) {
                throw new ArgumentException("No DomainEvent types provided this handler can handle.", nameof(recognizedTypes));
            }
            RecognizedTypes = recognizedTypes.ToList().AsReadOnly();
        }

        public bool CanHandle(DomainEvent evt) => RecognizedTypes.Contains(evt.Type);

        public abstract void Handle(DomainEvent evt);
    }

    public abstract class DomainEventHandler<T> : DomainEventHandler where T : DomainEvent {
        public DomainEventHandler(params string[] registeredNames) : base(registeredNames) { }

        public override void Handle(DomainEvent evt) {
            if (!(evt is T typedEvent)) {
                throw new ArgumentException("Whut? Cannot handle this event.", nameof(evt));
            }
            Handle(typedEvent);
        }

        public abstract void Handle(T evt);
    }
}
