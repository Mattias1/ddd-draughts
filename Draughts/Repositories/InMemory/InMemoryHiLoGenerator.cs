using System.Linq;

namespace Draughts.Repositories.InMemory {
    /// <summary>
    /// A thread safe id generator that doesn't need 'database connection' most of the time.
    /// Should be injected as singleton.
    /// </summary>
    public class InMemoryHiLoGenerator : BaseHiLoGenerator, IIdGenerator {
        public InMemoryHiLoGenerator(int intervalSize) : base(intervalSize) { }

        protected override HiLoInterval ReserveNewInterval() {
            var availableId = MiscDatabase.IdGenerationTable.Single();
            long lo = availableId.Id;

            availableId.Id += IntervalSize;

            return new HiLoInterval(lo, lo + IntervalSize);
        }
    }
}
