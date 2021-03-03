using FluentAssertions;
using Xunit;
using SqlQueryBuilder.Common;

namespace SqlQueryBuilder.Test.Common {
    public class IReadOnlyListExtensionsTest {
        [Fact]
        public void TestUnpackDuo() {
            var numbers = new int[] { 42, 1337 };

            var (first, second) = numbers.UnpackDuo();

            first.Should().Be(42);
            second.Should().Be(1337);
        }

        [Fact]
        public void TestUnpackTrio() {
            var numbers = new int[] { 8, 7, 56 };

            var (first, second, third) = numbers.UnpackTrio();

            first.Should().Be(8);
            second.Should().Be(7);
            third.Should().Be(56);
        }

        [Fact]
        public void TestUnpackQuad() {
            var chars = new char[] { 'c', 'h', 'a', 'r' };

            var (first, second, third, fourth) = chars.UnpackQuad();

            first.Should().Be('c');
            second.Should().Be('h');
            third.Should().Be('a');
            fourth.Should().Be('r');
        }
    }
}
