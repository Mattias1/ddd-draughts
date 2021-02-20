using Draughts.Repositories.InMemory;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Repositories.InMemory {
    public class InMemoryHiLoGeneratorTest {
        private readonly InMemoryHiLoGenerator _idGenerator;

        public InMemoryHiLoGeneratorTest() {
            _idGenerator = new InMemoryHiLoGenerator(3);
            MiscDatabase.IdGenerationTable[0].AvailableId = 1;
        }

        [Fact]
        public void IdGeneratorTest() {
            var idPool = _idGenerator.ReservePool(1);
            idPool.Count.Should().Be(3);
            idPool.Next().Should().Be(1);
            idPool.Next().Should().Be(2);
            idPool.Count.Should().Be(1);

            idPool = _idGenerator.ReservePool(2);
            idPool.Count.Should().Be(4);
            idPool.Next().Should().Be(3);
            idPool.Next().Should().Be(4);
            idPool.Next().Should().Be(5);
            idPool.Next().Should().Be(6);
            idPool.Count.Should().Be(0);
        }
    }
}
