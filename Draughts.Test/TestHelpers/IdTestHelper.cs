using Draughts.Repositories;
using Draughts.Repositories.Misc;
using System.Collections.Generic;

namespace Draughts.Test.TestHelpers;

public static class IdTestHelper {
    public static IIdGenerator IdGenerator { get; set; } = BuildFakeGenerator();

    public static long Next() => IdGenerator.ReservePool(1, 1, 1).Next();
    public static long NextForGame() => IdGenerator.ReservePool(1, 1, 1).NextForGame();
    public static long NextForUser() => IdGenerator.ReservePool(1, 1, 1).NextForUser();

    public static FakeIdGenerator BuildFakeGenerator() => new FakeIdGenerator();

    public sealed class FakeIdGenerator : IIdGenerator {
        private FakePool _pool = new FakePool();
        public IIdPool ReservePool() => _pool;
        public IIdPool ReservePool(int minimumSizeMisc, int minimumSizeGame, int minimumSizeUser) => ReservePool();
    }

    public sealed class FakePool : IIdPool {
        private Dictionary<string, long> _availableIds = new Dictionary<string, long>(3) {
                { DbIdGeneration.SUBJECT_MISC, 1 }, { DbIdGeneration.SUBJECT_GAME, 1 }, { DbIdGeneration.SUBJECT_USER, 1 }
            };

        public int Count() => 42;
        public int CountForGame() => 42;
        public int CountForUser() => 42;

        public long Next() => Next(DbIdGeneration.SUBJECT_MISC);
        public long NextForGame() => Next(DbIdGeneration.SUBJECT_GAME);
        public long NextForUser() => Next(DbIdGeneration.SUBJECT_USER);
        public long Next(string subject) => _availableIds[subject]++;
    }
}
