using System.Threading.Tasks;

namespace Puzzle.Screens
{
    public interface IScreenManager
    {
        Task ShowAsync(ScreenId screenId, object payload = null);

        Task HideAsync(ScreenId screenId);

        Task HideAllAsync();
    }
}