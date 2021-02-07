namespace Draughts.Repositories.Database {
    /// <summary>
    /// A thread safe id generator that doesn't need database connection most of the time.
    /// Should be injected as singleton.
    /// </summary>
    public class DbHiLoGenerator : BaseHiLoGenerator, IIdGenerator {
        public DbHiLoGenerator(int intervalSize) : base(intervalSize) { }

        protected override HiLoInterval ReserveNewInterval() {
            using (var tranFlavor = DbContext.Get.MiscTransaction()) {
                var idGenerationRow = DbContext.Get.Query(tranFlavor).SelectAllFrom("id_generation").Single<DbIdGeneration>();
                var lo = idGenerationRow.AvailableId;

                idGenerationRow.AvailableId += IntervalSize;
                DbContext.Get.Query(tranFlavor).InsertInto("id_generation").InsertFrom(idGenerationRow);

                tranFlavor.Commit();

                return new HiLoInterval(lo, lo + IntervalSize);
            }
        }
    }
}
