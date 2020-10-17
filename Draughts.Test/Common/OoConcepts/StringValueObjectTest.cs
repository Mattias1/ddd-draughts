using Draughts.Common.OoConcepts;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Common.OoConcepts {
    public class StringValueObjectTest {
        [Fact]
        public void Entities_ShouldBeEqual_WhenValuesAreEqual() {
            var left = new TestString("Howdy?");
            var right = new TestString("Howdy?");

            left.Equals(right as object).Should().BeTrue();
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [Fact]
        public void Entities_ShouldBeEqual_WhenValuesHaveDifferentCasing() {
            var left = new TestString("CASELESS");
            var right = new TestString("CaSeLeSS");

            left.Equals(right as object).Should().BeTrue();
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [Fact]
        public void Entities_ShouldBeEqual_WhenContainingWeirdCharacters() {
            var left = new TestString("Iñtërnâtiônàlizætiøn_𐐒𐐌_あ");
            var right = new TestString("Iñtërnâtiônàlizætiøn_𐐒𐐌_あ");

            left.Equals(right as object).Should().BeTrue();
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [Fact]
        public void Entities_ShouldNotBeEqual_WhenValuesAreDifferent() {
            var left = new TestString("left");
            var right = new TestString("right");

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [Fact]
        public void Entities_ShouldNotBeEqual_WhenTheOtherIsNull() {
            var left = new TestString("left");
            TestString? right = null;

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();

            (right == left).Should().BeFalse();
            (right != left).Should().BeTrue();
        }

        [Fact]
        public void Entities_ShouldBeEqual_WhenBothAreNull() {
            TestString? left = null;
            TestString? right = null;

            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
        }

        public class TestString : StringValueObject<TestString> {
            public override string Value { get; }

            public TestString(string value) => Value = value;
        }
    }
}
