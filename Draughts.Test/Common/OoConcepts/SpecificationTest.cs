using Draughts.Common.OoConcepts;
using Draughts.Test.Fakes;
using FluentAssertions;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using static Draughts.Common.OoConcepts.Specification<int>;
using static Draughts.Repositories.Database.JoinEnum;

namespace Draughts.Test.Common.OoConcepts {
    public class SpecificationTest {
        private IInitialQueryBuilder Query() => QueryBuilder.Init(new FakeSqlFlavor());

        [Theory]
        [InlineData(-1, -1, false)]
        [InlineData(1, -1, false)]
        [InlineData(-1, 1, false)]
        [InlineData(1, 1, true)]
        public void TestAndSpecification(int a, int b, bool result) {
            var aSpec = new PositiveSignSpecification(a);
            var bSpec = new PositiveSignSpecification(b);
            aSpec.And(bSpec).IsSatisfiedBy(42).Should().Be(result);
        }

        [Theory]
        [InlineData(-1, -1, false)]
        [InlineData(1, -1, true)]
        [InlineData(-1, 1, true)]
        [InlineData(1, 1, true)]
        public void TestOrSpecification(int a, int b, bool result) {
            var aSpec = new PositiveSignSpecification(a);
            var bSpec = new PositiveSignSpecification(b);
            aSpec.Or(bSpec).IsSatisfiedBy(42).Should().Be(result);
        }

        [Theory]
        [InlineData(-1, true)]
        [InlineData(1, false)]
        public void TestNotSpecification(int input, bool result) {
            var spec = new PositiveSignSpecification(input);
            spec.Not().IsSatisfiedBy(42).Should().Be(result);
        }

        [Fact]
        public void TestAndSpecificationQuery(){
            var aSpec = new PositiveSignSpecification(1);
            var bSpec = new PositiveSignSpecification(-1);

            var q = Query().SelectAllFrom("test");

            aSpec.And(bSpec).ApplyQueryBuilder(q);

            q.ToUnsafeSql().Should().Be("select test.* from test where number > 0 and number < 0");
        }

        [Fact]
        public void TestOrSpecificationQuery(){
            var aSpec = new PositiveSignSpecification(1);
            var bSpec = new PositiveSignSpecification(-1);

            var q = Query().SelectAllFrom("test");

            aSpec.Or(bSpec).ApplyQueryBuilder(q);

            q.ToUnsafeSql().Should().Be("select test.* from test where number > 0 or number < 0");
        }

        [Fact]
        public void TestNotSpecificationQuery(){
            var spec = new PositiveSignSpecification(1);
            var q = Query().SelectAllFrom("test");

            spec.Not().ApplyQueryBuilder(q);

            q.ToUnsafeSql().Should().Be("select test.* from test where not (number > 0)");
        }

        [Fact]
        public void TestAndSpecificationJoins(){
            var aSpec = new AuthUserRoleJoinSpecification();
            var bSpec = new RoleJoinSpecification();

            var joins = aSpec.And(bSpec).RequiredJoins();

            joins.Should().BeEquivalentTo(PossibleJoins.AuthUserRole, PossibleJoins.Role);
        }

        [Fact]
        public void TestOrSpecificationJoins(){
            var aSpec = new AuthUserRoleJoinSpecification();
            var bSpec = new RoleJoinSpecification();

            var joins = aSpec.Or(bSpec).RequiredJoins();

            joins.Should().BeEquivalentTo(PossibleJoins.AuthUserRole, PossibleJoins.Role);
        }

        [Fact]
        public void TestNotSpecificationJoins(){
            var spec = new AuthUserRoleJoinSpecification();

            var joins = spec.Not().RequiredJoins();

            joins.Should().BeEquivalentTo(PossibleJoins.AuthUserRole);
        }

        private class PositiveSignSpecification : Specification<int> {
            private readonly int _sign;
            public PositiveSignSpecification(int sign) => _sign = sign;
            public override Expression<Func<int, bool>> ToExpression() => i => i * _sign > 0;
            public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
                if (_sign > 0) {
                    ApplyColumnWhere(builder, whereType, "number", q => q.Gt(0));
                }
                else {
                    ApplyColumnWhere(builder, whereType, "number", q => q.Lt(0));
                }
            }
        }

        private class AuthUserRoleJoinSpecification : Specification<int> {
            public override Expression<Func<int, bool>> ToExpression() => i => i == 0;
            public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
                ApplyColumnWhere(builder, whereType, "column-aur", q => q.Is(0));
            }
            public override IEnumerable<PossibleJoins> RequiredJoins() {
                yield return PossibleJoins.AuthUserRole;
            }
        }

        private class RoleJoinSpecification : Specification<int> {
            public override Expression<Func<int, bool>> ToExpression() => i => i == 0;
            public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
                ApplyColumnWhere(builder, whereType, "column-r", q => q.Is(0));
            }
            public override IEnumerable<PossibleJoins> RequiredJoins() {
                yield return PossibleJoins.Role;
            }
        }
    }
}
