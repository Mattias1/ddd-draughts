using System.Collections.Generic;

namespace Draughts.Repositories.Misc;

/// <summary>
/// A thread safe id generator that doesn't need a database connection most of the time.
/// Should be injected as singleton.
/// </summary>
public sealed class HiLoIdGenerator : IIdGenerator {
    private readonly Dictionary<string, HiLoGenerator> _generators;

    private HiLoIdGenerator(HiLoGenerator miscGenerator,
            HiLoGenerator gameGenerator, HiLoGenerator userGenerator) {
        _generators = new Dictionary<string, HiLoGenerator> {
                { miscGenerator.Subject, miscGenerator },
                { gameGenerator.Subject, gameGenerator },
                { userGenerator.Subject, userGenerator }
            };
    }

    private int Count(string subject) => _generators[subject].Count;

    private long Next(string subject) => _generators[subject].Next();

    public IIdPool ReservePool() => ReservePool(20, 1, 1);
    public IIdPool ReservePool(int minimumSizeMisc, int minimumSizeGame, int minimumSizeUser) {
        _generators[DbIdGeneration.SUBJECT_MISC].ReservePool(minimumSizeMisc);
        _generators[DbIdGeneration.SUBJECT_GAME].ReservePool(minimumSizeGame);
        _generators[DbIdGeneration.SUBJECT_USER].ReservePool(minimumSizeUser);
        return new HiLoPool(this);
    }

    public static HiLoIdGenerator BuildHiloGIdGenerator(int miscInterval, int gameInterval, int userInterval) {
        return new HiLoIdGenerator(
            new HiLoGenerator(miscInterval, DbIdGeneration.SUBJECT_MISC),
            new HiLoGenerator(gameInterval, DbIdGeneration.SUBJECT_GAME),
            new HiLoGenerator(userInterval, DbIdGeneration.SUBJECT_USER)
        );
    }

    public struct HiLoPool : IIdPool {
        private readonly HiLoIdGenerator _generators;

        public int Count() => _generators.Count(DbIdGeneration.SUBJECT_MISC);
        public int CountForGame() => _generators.Count(DbIdGeneration.SUBJECT_GAME);
        public int CountForUser() => _generators.Count(DbIdGeneration.SUBJECT_USER);

        public long Next() => _generators.Next(DbIdGeneration.SUBJECT_MISC);
        public long NextForGame() => _generators.Next(DbIdGeneration.SUBJECT_GAME);
        public long NextForUser() => _generators.Next(DbIdGeneration.SUBJECT_USER);

        internal HiLoPool(HiLoIdGenerator generators) {
            _generators = generators;
        }
    }
}
