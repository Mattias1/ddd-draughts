using System.Collections.Generic;

namespace Draughts.Common.Events {
    public interface IEventQueue {
        void Register(IDomainEventHandler eventHandler);
        void Raise(DomainEvent evt);
        bool HandleNext();
        void HandleAll();
    }
}