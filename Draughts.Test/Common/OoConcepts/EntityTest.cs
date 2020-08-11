using Draughts.Common.OoConcepts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Draughts.Test.Common.OoConcepts {
    [TestClass]
    public class EntityTest {
        [TestMethod]
        public void Entities_ShouldBeEqual_WhenIdsAreEqual() {
            var left = new TestEntity(new TestId(1), "left");
            var right = new TestEntity(new TestId(1), "right");

            left.Equals(right as object).Should().BeTrue();
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [TestMethod]
        public void Entities_ShouldNotBeEqual_WhenIdsAreDifferent() {
            var left = new TestEntity(new TestId(1), "whatever");
            var right = new TestEntity(new TestId(2), "whatever");

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [TestMethod]
        public void Entities_ShouldNotBeEqual_WhenTheOtherIsNull() {
            var left = new TestEntity(new TestId(1), "left");
            TestEntity? right = null;

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();

            (right == left).Should().BeFalse();
            (right != left).Should().BeTrue();
        }

        [TestMethod]
        public void Entities_ShouldBeEqual_WhenBothAreNull() {
            TestEntity? left = null;
            TestEntity? right = null;

            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
        }

        public class TestId : IdValueObject<TestId> {
            public override long Id { get; }

            public TestId(long id) => Id = id;
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
}
