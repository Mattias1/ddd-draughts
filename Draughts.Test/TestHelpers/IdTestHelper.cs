using Draughts.Repositories.Database;
using System.Collections.Generic;

namespace Draughts.Test.TestHelpers {
    public class IdTestHelper {
        private static Dictionary<string, long> _availableIds = new Dictionary<string, long>(3) {
            { DbIdGeneration.SUBJECT_MISC, 1 }, { DbIdGeneration.SUBJECT_GAME, 1 }, { DbIdGeneration.SUBJECT_USER, 1 }
        };

        public static long Next() => Next(DbIdGeneration.SUBJECT_MISC);
        public static long NextGame() => Next(DbIdGeneration.SUBJECT_GAME);
        public static long NextUser() => Next(DbIdGeneration.SUBJECT_USER);
        public static long Next(string subject) => _availableIds[subject]++;

        public static void Seed(long id) => Seed(DbIdGeneration.SUBJECT_MISC, id);
        public static void Seed(string subject, long id) => _availableIds[subject] = id;
        public static void Seed(long id, long gameId, long userId) {
            Seed(DbIdGeneration.SUBJECT_MISC, id);
            Seed(DbIdGeneration.SUBJECT_GAME, gameId);
            Seed(DbIdGeneration.SUBJECT_USER, userId);
        }
    }
}
