using Draughts.Repositories.Database;
using Draughts.Repositories.InMemory;
using System;
using System.Collections.Generic;

namespace Draughts.Repositories {
    /// <summary>
    /// A thread safe id generator that doesn't need a database connection most of the time.
    /// Should be injected as singleton.
    /// </summary>
    public class HiLoIdGenerator : IIdGenerator {
        private readonly Dictionary<string, BaseHiLoGenerator> _generators;

        private HiLoIdGenerator(BaseHiLoGenerator miscGenerator,
                BaseHiLoGenerator gameGenerator, BaseHiLoGenerator userGenerator) {
            _generators = new Dictionary<string, BaseHiLoGenerator> {
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

        public static HiLoIdGenerator InMemoryHiloGIdGenerator(int miscInterval, int gameInterval, int userInterval) {
            return new HiLoIdGenerator(
                new InMemoryHiLoGenerator(miscInterval, DbIdGeneration.SUBJECT_MISC),
                new InMemoryHiLoGenerator(gameInterval, DbIdGeneration.SUBJECT_GAME),
                new InMemoryHiLoGenerator(userInterval, DbIdGeneration.SUBJECT_USER)
            );
        }

        public static HiLoIdGenerator DbHiloGIdGenerator(int miscInterval, int gameInterval, int userInterval) {
            return new HiLoIdGenerator(
                new DbHiLoGenerator(miscInterval, DbIdGeneration.SUBJECT_MISC),
                new DbHiLoGenerator(gameInterval, DbIdGeneration.SUBJECT_GAME),
                new DbHiLoGenerator(userInterval, DbIdGeneration.SUBJECT_USER)
            );
        }

        public struct HiLoPool : IIdPool {
            private readonly HiLoIdGenerator _generators;

            public int Count() => _generators.Count(DbIdGeneration.SUBJECT_MISC);
            public int CountGame() => _generators.Count(DbIdGeneration.SUBJECT_GAME);
            public int CountUser() => _generators.Count(DbIdGeneration.SUBJECT_USER);

            public long Next() => _generators.Next(DbIdGeneration.SUBJECT_MISC);
            public long NextGame() => _generators.Next(DbIdGeneration.SUBJECT_GAME);
            public long NextUser() => _generators.Next(DbIdGeneration.SUBJECT_USER);

            internal HiLoPool(HiLoIdGenerator generators) {
                _generators = generators;
            }
        }
    }
}
