using Draughts.Common.OoConcepts;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Common.Events.Specifications;

public sealed class EventIdSort : Sort<DomainEvent, DomainEventId> {
    public EventIdSort() : base(defaultDescending: true) { }
    public override Expression<Func<DomainEvent, DomainEventId>> ToExpression() => a => a.Id;
    public override IQueryBuilder ApplyQueryBuilder(IQueryBuilder builder) => ApplyColumnSort(builder, "sent_events.id");
}
