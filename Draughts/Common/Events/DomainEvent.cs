namespace Draughts.Common.Events {
    public abstract class DomainEvent {
        public string Type { get; }

        // TODO: Unique id, created, last attempt and nr of tries properties

        public DomainEvent(string type) => Type = type;
    }
}
