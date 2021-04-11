using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class AdminLogIdSort : Sort<AdminLog, AdminLogId> {
        public AdminLogIdSort() : base(defaultDescending: true) { }
        public override Expression<Func<AdminLog, AdminLogId>> ToExpression() => a => a.Id;
        public override IQueryBuilder ApplyQueryBuilder(IQueryBuilder builder) => ApplyColumnSort(builder, "adminlog.id");
    }
}
