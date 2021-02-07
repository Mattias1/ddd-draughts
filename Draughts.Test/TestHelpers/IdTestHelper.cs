namespace Draughts.Test.TestHelpers {
    public class IdTestHelper {
        private static long _availableId = 1;
        public static long Next() => _availableId++;

        public static void Seed(long id) => _availableId = id;
    }
}
