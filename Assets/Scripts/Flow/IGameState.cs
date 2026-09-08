using System.Threading.Tasks;

namespace Puzzle.Flow
{
    public interface IGameState
    {
        Task OnEnterAsync();
        Task OnExitAsync();
    }
}