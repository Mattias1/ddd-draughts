using FluentAssertions;
using Xunit;
using System.Linq;
using SqlQueryBuilder.Common;

namespace SqlQueryBuilder.Test.Common {
    public class IEnumerableExtensionsTest {
        [Fact]
        public void TestChunk() {
            var fibbonacci = new int[] { 1, 1, 2, 3, 5, 8, 13, 21 };

            var result = fibbonacci.Chunk(3).ToList();

            result.Count.Should().Be(3);
            result[0].Should().BeEquivalentTo(new int[] { 1, 1, 2 });
            result[1].Should().BeEquivalentTo(new int[] { 3, 5, 8 });
            result[2].Should().BeEquivalentTo(new int[] { 13, 21 });
        }
    }
}
