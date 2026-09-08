namespace Puzzle.Services
{
    public interface ISaveService
    {
        void SaveInt(string key, int value);

        int LoadInt(string key, int defaultValue = 0);

        bool HasKey(string key);
    }
}