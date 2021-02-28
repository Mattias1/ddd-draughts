namespace Draughts.Repositories {
    public interface IIdPool {
        int Count();
        int CountForGame();
        int CountForUser();

        long Next();
        long NextForGame();
        long NextForUser();
    }
}
