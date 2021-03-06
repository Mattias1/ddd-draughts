using Draughts.Common.OoConcepts;

namespace Draughts.Common.Events {
    public class DomainEventId : IdValueObject<DomainEventId> {
        public override long Id { get; }

        public DomainEventId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid domain event id.");
            }
            Id = id;
        }
    }
}
