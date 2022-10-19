using Draughts.Common.Utilities;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.Events;

public abstract class DomainEventHandlerBase : IDomainEventHandler {
    private readonly IClock _clock;
    private readonly EventsRepository _eventsRepository;
    private readonly IUnitOfWork _unitOfWork;

    protected DomainEventHandlerBase(IClock clock, EventsRepository eventsRepository, IUnitOfWork unitOfWork) {
        _clock = clock;
        _eventsRepository = eventsRepository;
        _unitOfWork = unitOfWork;
    }

    public abstract bool CanHandle(DomainEvent evt);
    public abstract void Handle(DomainEvent evt);

    protected void HandleWithTransaction(TransactionDomain transactionDomain, DomainEvent evt,
            Action<ITransaction> eventHandlerFunction) {
        ZonedDateTime handledAt = _clock.UtcNow();
        _unitOfWork.WithTransaction(transactionDomain, tran => {
            if (_eventsRepository.EventIsReceived(evt.Id)) {
                // This event is already received and handled, no need to do anything.
                // The sender will eventually notice and stop asking us to handle this one :].
                return;
            }

            eventHandlerFunction(tran);

            _eventsRepository.MarkEventAsReceived(evt.Id, handledAt);
        });

        // Let's already try to make the sender stop asking us now. Just for efficiency.
        // If this fails, no problems, we'll catch up later.
        _unitOfWork.WithTransaction(evt.OriginTransactionDomain, tran => {
            _eventsRepository.MarkEventAsHandled(evt.Id, handledAt);
        });
    }
}

public abstract class DomainEventHandler : DomainEventHandlerBase {
    protected IReadOnlyList<string> RecognizedTypes { get; }

    protected DomainEventHandler(IClock clock, EventsRepository eventsRepository, IUnitOfWork unitOfWork,
            params string[] recognizedTypes) : base(clock, eventsRepository, unitOfWork) {
        if (recognizedTypes.Length == 0) {
            throw new ArgumentException("No DomainEvent types provided this handler can handle.", nameof(recognizedTypes));
        }
        RecognizedTypes = recognizedTypes.ToList().AsReadOnly();
    }

    public override bool CanHandle(DomainEvent evt) => RecognizedTypes.Contains(evt.Type);
}

public abstract class DomainEventHandler<T> : DomainEventHandlerBase where T : DomainEvent {
    protected DomainEventHandler(IClock clock, EventsRepository eventsRepository, IUnitOfWork unitOfWork)
        : base(clock, eventsRepository, unitOfWork) { }

    public override bool CanHandle(DomainEvent evt) => evt is T;

    public override void Handle(DomainEvent evt) {
        if (evt is not T typedEvent) {
            throw new ArgumentException("Whut? Cannot handle this event.", nameof(evt));
        }
        Handle(typedEvent);
    }

    public abstract void Handle(T evt);
}
