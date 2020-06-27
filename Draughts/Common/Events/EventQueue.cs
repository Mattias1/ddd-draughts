using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.Events {
    public class EventQueue : IEventQueue {
        private readonly List<IDomainEventHandler> _eventHandlers;
        private readonly Queue<DomainEvent> _queue;

        public EventQueue() {
            _eventHandlers = new List<IDomainEventHandler>();
            _queue = new Queue<DomainEvent>();
        }

        public void Register(IDomainEventHandler eventHandler) => _eventHandlers.Add(eventHandler);

        public void Raise(DomainEvent evt) {
            _queue.Enqueue(evt);
            // TODO: In a different thread?
            HandleNext();
        }

        public bool HandleNext() {
            var evt = _queue.Dequeue();
            try {
                bool foundOne = false;
                foreach (var handler in _eventHandlers.Where(h => h.CanHandle(evt))) {
                    handler.Handle(evt);
                    foundOne = true;
                }
                return foundOne;
            }
            catch (Exception e) {
                // TODO: Put this in a try later / failed queue or something.
                _queue.Enqueue(evt);
                throw;
                // return false;
            }
        }

        public void HandleAll() {
            throw new NotImplementedException("TODO");
        }
    }
}
