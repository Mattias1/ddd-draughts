using Draughts.Common.OoConcepts;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Common.OoConcepts;

public class EntityTest {
    [Fact]
    public void EqualWhenIdsAreEqual() {
        var left = new TestEntity(new TestId(1), "left");
        var right = new TestEntity(new TestId(1), "right");

        left.Equals(right as object).Should().BeTrue();
        left.Equals(right).Should().BeTrue();
        (left == right).Should().BeTrue();
        (left != right).Should().BeFalse();
        left.GetHashCode().Should().Be(right.GetHashCode());
    }

    [Fact]
    public void NotEqualWhenIdsAreDifferent() {
        var left = new TestEntity(new TestId(1), "whatever");
        var right = new TestEntity(new TestId(2), "whatever");

        left.Equals(right as object).Should().BeFalse();
        left.Equals(right).Should().BeFalse();
        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();
    }

    [Fact]
    public void NotEqualWhenTheOtherIsNull() {
        var left = new TestEntity(new TestId(1), "left");
        TestEntity? right = null;

        left.Equals(right as object).Should().BeFalse();
        left.Equals(right).Should().BeFalse();
        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();

        (right == left).Should().BeFalse();
        (right != left).Should().BeTrue();
    }

    [Fact]
    public void EqualWhenBothAreNull() {
        TestEntity? left = null;
        TestEntity? right = null;

        (left == right).Should().BeTrue();
        (left != right).Should().BeFalse();
    }

    public class TestId : IdValueObject<TestId> {
        public override long Value { get; }

        public TestId(long id) => Value = id;
    }

    public class TestEntity : Entity<TestEntity, TestId> {
        public override TestId Id { get; }
        public string Whatever { get; }

        public TestEntity(TestId id, string whatever) {
            Id = id;
            Whatever = whatever;
        }
    }
}
