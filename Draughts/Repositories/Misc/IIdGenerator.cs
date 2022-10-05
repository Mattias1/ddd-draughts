namespace Draughts.Repositories.Misc;

public interface IIdGenerator {
    IIdPool ReservePool();
    IIdPool ReservePool(int minimumSizeMisc, int minimumSizeGame, int minimumSizeUser);
}
