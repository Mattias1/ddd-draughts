using Draughts.Common.OoConcepts;
using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Testing;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Draughts.Test.Common.OoConcepts;

public sealed class SortTest {
    private IInitialQueryBuilder Query() => QueryBuilder.Init(new FakeSqlFlavor());

    [Fact]
    public void TestSortQuery() {
        var sort = new SortTestSort();
        var q = Query().SelectAllFrom("test");

        sort.ApplyQueryBuilder(q);

        q.ToUnsafeSql().Should().Be("select `test`.* from `test` order by `value` desc");
    }

    private sealed class SortTestSort : Sort<SortTestNumber, int> {
        public SortTestSort() : base(defaultDescending: true) { }
        public override Expression<Func<SortTestNumber, int>> ToExpression() => n => n.Value;
        public override IQueryBuilder ApplyQueryBuilder(IQueryBuilder builder) => ApplyColumnSort(builder, "value");
    }

    private sealed class SortTestNumber {
        public int Value { get; set; }
    }
}
