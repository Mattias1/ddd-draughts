namespace Draughts.Repositories.Database;

/// <summary>
/// A thread safe id generator that doesn't need a database connection most of the time.
/// Should be injected as singleton.
/// </summary>
public sealed class DbHiLoGenerator : BaseHiLoGenerator {
    public DbHiLoGenerator(int intervalSize, string subject) : base(intervalSize, subject) { }

    protected override HiLoInterval ReserveNewInterval() {
        using (var tranFlavor = DbContext.Get.BeginMiscTransaction()) {
            var idGenerationRow = DbContext.Get.Query(tranFlavor)
                .SelectAllFrom("id_generation")
                .Where("subject").Is(Subject)
                .Single<DbIdGeneration>();
            var lo = idGenerationRow.AvailableId;

            idGenerationRow.AvailableId += IntervalSize;
            DbContext.Get.Query(tranFlavor)
                .Update("id_generation")
                .SetFrom(idGenerationRow)
                .Where("subject").Is(Subject)
                .Execute();

            tranFlavor.Commit();

            return new HiLoInterval(lo, lo + IntervalSize);
        }
    }
}
