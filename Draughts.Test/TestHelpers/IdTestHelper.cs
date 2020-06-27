using Draughts.Repositories;
using System.Collections.Generic;

namespace Draughts.Test.TestHelpers {
    public class IdTestHelper {
        private static readonly List<AvailableId> AvailableIdsTable = new List<AvailableId>(1) { new AvailableId() };
        private static readonly IIdGenerator IdGenerator = new LoHiGenerator(1, () => AvailableIdsTable);

        public static long Next() => IdGenerator.Next();
    }
}
