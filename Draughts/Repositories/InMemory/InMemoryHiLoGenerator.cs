using System.Linq;

namespace Draughts.Repositories.InMemory {
    /// <summary>
    /// A thread safe id generator that doesn't need 'database connection' most of the time.
    /// Should be injected as singleton.
    /// </summary>
    public class InMemoryHiLoGenerator : BaseHiLoGenerator, IIdGenerator {
        public InMemoryHiLoGenerator(int intervalSize) : base(intervalSize) { }

        protected override HiLoInterval ReserveNewInterval() {
            var idGenerationColumn = MiscDatabase.IdGenerationTable.Single();
            long lo = idGenerationColumn.AvailableId;

            idGenerationColumn.AvailableId += IntervalSize;

            return new HiLoInterval(lo, lo + IntervalSize);
        }
    }
}
