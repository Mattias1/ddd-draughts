namespace Draughts.Repositories {
    public interface IIdGenerator {
        IIdPool ReservePool();
        IIdPool ReservePool(int minimumSize);
    }
}