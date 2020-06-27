using Draughts.Common;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Draughts.Test.Common {
    [TestClass]
    public class IEnumerableExtensionsTest {
        [TestMethod]
        [DataRow(new[] { 1, 1, 2, 3, 5, 8 }, new[] { 3, 8 }, true)]
        [DataRow(new[] { 1, 1, 2, 3, 5, 8 }, new[] { 4, 6 }, false)]
        [DataRow(new[] { 1, 1, 2, 3, 5, 8 }, new[] { 3, 8, 13 }, false)]
        [DataRow(new[] { 1, 1, 2, 3, 5, 8 }, new[] { 1, 1, 1 }, true)]
        [DataRow(new[] { 1, 1, 2, 3, 5, 8 }, new int[0], true)]
        [DataRow(new int[0], new[] { 3, 8 }, false)]
        [DataRow(new int[0], new int[0], true)]
        public void TestContainsAll(int[] haystack, int[] needles, bool expectedResult) {
            haystack.ContainsAll(needles).Should().Be(expectedResult);
        }
    }
}
