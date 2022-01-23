using Draughts.Common.OoConcepts;
using Draughts.Domain.UserContext.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.UserContext.Specifications;

public class UserUsernameSpecification : Specification<User> {
    private readonly string? _username;

    public UserUsernameSpecification(string? username) => _username = username;

    public override Expression<Func<User, bool>> ToExpression() => u => u.Username == _username;

    public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
        ApplyColumnWhere(builder, whereType, "username", q => q.Is(_username));
    }
}
