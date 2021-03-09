using System.Linq;

namespace Draughts.Repositories.InMemory {
    /// <summary>
    /// A thread safe id generator that doesn't need a 'database connection' most of the time.
    /// Should be injected as singleton.
    /// </summary>
    public class InMemoryHiLoGenerator : BaseHiLoGenerator {
        public InMemoryHiLoGenerator(int intervalSize, string subject) : base(intervalSize, subject) { }

        protected override HiLoInterval ReserveNewInterval() {
            var idGenerationRow = MiscDatabase.Get.IdGenerationTable.Single(t => t.Subject == Subject);
            long lo = idGenerationRow.AvailableId;

            idGenerationRow.AvailableId += IntervalSize;

            return new HiLoInterval(lo, lo + IntervalSize);
        }
    }
}
