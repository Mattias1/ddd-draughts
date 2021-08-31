using Draughts.Common.OoConcepts;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Draughts.Test.Common.OoConcepts {
    public class ValueObjectTest {
        [Fact]
        public void EqualWhenComponentsAreEqual() {
            var left = new TestValueObject(1, "caseless", "ABcd");
            var right = new TestValueObject(1, "caseLESS", "ABcd");

            left.Equals(right as object).Should().BeTrue();
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [Theory]
        [InlineData(1, "caseless", "ABcd", 2, "caseless", "ABcd")]
        [InlineData(1, "caseless", "ABcd", 1, "different", "ABcd")]
        [InlineData(1, "caseless", "ABcd", 1, "caseless", "abCD")]
        public void NotEqualWhenComponentsAreDifferent(
            int leftId, string leftCaseless, string leftCaseSensitive,
            int rightId, string rightCaseless, string rightCaseSensitive
        ) {
            var left = new TestValueObject(leftId, leftCaseless, leftCaseSensitive);
            var right = new TestValueObject(rightId, rightCaseless, rightCaseSensitive);

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [Fact]
        public void NotEqualWhenTheOtherIsNull() {
            var left = new TestValueObject(1, "caseless", "Blah");
            TestValueObject? right = null;

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();

            (right == left).Should().BeFalse();
            (right != left).Should().BeTrue();
        }

        [Fact]
        public void EqualWhenBothAreNull() {
            TestValueObject? left = null;
            TestValueObject? right = null;

            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
        }

        public class TestValueObject : ValueObject<TestValueObject> {
            public int Number { get; }
            public string CaseInsensitiveString { get; }
            public string CaseSensitiveString { get; }

            public TestValueObject(int nr, string caseless, string cAsESEnsiTIv3) {
                Number = nr;
                CaseInsensitiveString = caseless;
                CaseSensitiveString = cAsESEnsiTIv3;
            }

            protected override IEnumerable<object> GetEqualityComponents() {
                yield return Number;
                yield return CaseInsensitiveString.ToLower();
                yield return CaseSensitiveString;
            }
        }

        public class TestId : IdValueObject<TestId> {
            public override long Value { get; }

            public TestId(long id) => Value = id;
        }
    }
}
