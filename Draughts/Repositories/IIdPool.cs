namespace Draughts.Repositories {
    public interface IIdPool {
        int Count { get; }
        long Next();
    }
}