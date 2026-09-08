using System.Threading.Tasks;

namespace Puzzle.Screens
{
    public interface IScreenManager
    {
        Task ShowAsync(ScreenId screenId);

        Task HideAsync(ScreenId screenId);

        Task HideAllAsync();
    }
}