using Draughts.Common.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Draughts.Test.Common.Utilities {
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

        [TestMethod]
        public void TestForEach() {
            var list = new List<TestItem> { new TestItem(1), new TestItem(2) };

            list.ForEach(item => item.Value += 10);

            list.Should().OnlyContain(item => item.Value >= 10);
        }

        public class TestItem {
            public TestItem(int value) => Value = value;
            public int Value { get; set; }
        }
    }
}
