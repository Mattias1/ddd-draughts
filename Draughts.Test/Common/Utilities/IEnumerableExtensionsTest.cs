using Draughts.Common.Utilities;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Test.Common.Utilities {
    public class IEnumerableExtensionsTest {
        [Theory]
        [InlineData(new[] { 1, 1, 2, 3, 5, 8 }, new[] { 3, 8 }, true)]
        [InlineData(new[] { 1, 1, 2, 3, 5, 8 }, new[] { 4, 6 }, false)]
        [InlineData(new[] { 1, 1, 2, 3, 5, 8 }, new[] { 3, 8, 13 }, false)]
        [InlineData(new[] { 1, 1, 2, 3, 5, 8 }, new[] { 1, 1, 1 }, true)]
        [InlineData(new[] { 1, 1, 2, 3, 5, 8 }, new int[0], true)]
        [InlineData(new int[0], new[] { 3, 8 }, false)]
        [InlineData(new int[0], new int[0], true)]
        public void TestContainsAll(int[] haystack, int[] needles, bool expectedResult) {
            haystack.ContainsAll(needles).Should().Be(expectedResult);
        }

        [Fact]
        public void TestForEach() {
            var list = new List<TestItem> { new TestItem(1), new TestItem(2) };

            list.ForEach(item => item.Value += 10);

            list.Should().OnlyContain(item => item.Value >= 10);
        }

        [Fact]
        public void TestIntersectBy() {
            var fibbonacci = new int[] { 1, 1, 2, 3, 5, 8, 13, 21 }.Select(i => new TestItem(i));
            var primes = new int[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };

            fibbonacci.IntersectBy(primes, t => t.Value).Select(t => t.Value).Should().BeEquivalentTo(new int[] { 1, 1, 2, 3, 5, 13 });
        }

        [Fact]
        public void TestChunk() {
            var fibbonacci = new int[] { 1, 1, 2, 3, 5, 8, 13, 21 };

            var result = fibbonacci.Chunk(3).ToList();

            result.Count.Should().Be(3);
            result[0].Should().BeEquivalentTo(new int[] { 1, 1, 2 });
            result[1].Should().BeEquivalentTo(new int[] { 3, 5, 8 });
            result[2].Should().BeEquivalentTo(new int[] { 13, 21 });
        }

        public class TestItem {
            public TestItem(int value) => Value = value;
            public int Value { get; set; }
        }
    }
}
