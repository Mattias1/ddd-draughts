namespace Draughts.Common.Events {
    public interface IDomainEventHandler {
        bool CanHandle(DomainEvent evt);
        void Handle(DomainEvent evt);
    }
}
