using Draughts.Common.OoConcepts;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Draughts.Test.Common.OoConcepts {
    public class SpecificationTest {
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

        private class PositiveSignSpecification : Specification<int> {
            private readonly int _sign;
            public PositiveSignSpecification(int sign) => _sign = sign;
            public override Expression<Func<int, bool>> ToExpression() => i => i * _sign > 0;
        }
    }
}
