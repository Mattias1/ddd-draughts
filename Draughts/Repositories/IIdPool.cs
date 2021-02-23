namespace Draughts.Repositories {
    public interface IIdPool {
        int Count();
        int CountGame();
        int CountUser();

        long Next();
        long NextGame();
        long NextUser();
    }
}
