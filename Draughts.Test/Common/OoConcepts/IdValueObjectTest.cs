using Draughts.Common.OoConcepts;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Common.OoConcepts;

public sealed class IdValueObjectTest {
    [Fact]
    public void EqualWhenIdsAreEqual() {
        var left = new TestId(1);
        var right = new TestId(1);

        left.Equals(right as object).Should().BeTrue();
        left.Equals(right).Should().BeTrue();
        (left == right).Should().BeTrue();
        (left != right).Should().BeFalse();
        left.GetHashCode().Should().Be(right.GetHashCode());
    }

    [Fact]
    public void NotEqualWhenIdsAreDifferent() {
        var left = new TestId(1);
        var right = new TestId(2);

        left.Equals(right as object).Should().BeFalse();
        left.Equals(right).Should().BeFalse();
        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();
    }

    [Fact]
    public void NotEqualWhenTheOtherIsNull() {
        var left = new TestId(1);
        TestId? right = null;

        left.Equals(right as object).Should().BeFalse();
        left.Equals(right).Should().BeFalse();
        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();

        (right == left).Should().BeFalse();
        (right != left).Should().BeTrue();
    }

    [Fact]
    public void EqualWhenBothAreNull() {
        TestId? left = null;
        TestId? right = null;

        (left == right).Should().BeTrue();
        (left != right).Should().BeFalse();
    }

    public sealed class TestId : IdValueObject<TestId> {
        public override long Value { get; }

        public TestId(long id) => Value = id;
    }
}
