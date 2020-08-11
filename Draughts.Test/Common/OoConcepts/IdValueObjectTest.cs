using Draughts.Common.OoConcepts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Draughts.Test.Common.OoConcepts {
    [TestClass]
    public class IdValueObjectTest {
        [TestMethod]
        public void Entities_ShouldBeEqual_WhenIdsAreEqual() {
            var left = new TestId(1);
            var right = new TestId(1);

            left.Equals(right as object).Should().BeTrue();
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [TestMethod]
        public void Entities_ShouldNotBeEqual_WhenIdsAreDifferent() {
            var left = new TestId(1);
            var right = new TestId(2);

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [TestMethod]
        public void Entities_ShouldNotBeEqual_WhenTheOtherIsNull() {
            var left = new TestId(1);
            TestId? right = null;

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();

            (right == left).Should().BeFalse();
            (right != left).Should().BeTrue();
        }

        [TestMethod]
        public void Entities_ShouldBeEqual_WhenBothAreNull() {
            TestId? left = null;
            TestId? right = null;

            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
        }

        public class TestId : IdValueObject<TestId> {
            public override long Id { get; }

            public TestId(long id) => Id = id;
        }
    }
}
