using Draughts.Repositories;
using Draughts.Repositories.InMemory;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Repositories.InMemory;

public sealed class InMemoryHiLoGeneratorTest {
    [Fact]
    public void IdGeneratorTest() {
        var idGenerator = HiLoIdGenerator.InMemoryHiloGIdGenerator(3, 3, 3);
        MiscDatabase.Get.IdGenerationTable[0].AvailableId = 1;

        var idPool = idGenerator.ReservePool(1, 1, 1);
        idPool.Count().Should().Be(3);
        idPool.Next().Should().Be(1);
        idPool.Next().Should().Be(2);
        idPool.Count().Should().Be(1);

        idPool = idGenerator.ReservePool(2, 2, 2);
        idPool.Count().Should().Be(4);
        idPool.Next().Should().Be(3);
        idPool.Next().Should().Be(4);
        idPool.Next().Should().Be(5);
        idPool.Next().Should().Be(6);
        idPool.Count().Should().Be(0);
    }
}
