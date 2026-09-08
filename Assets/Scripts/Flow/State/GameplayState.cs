using System.Threading.Tasks;

namespace Puzzle.Flow
{
    public sealed class GameplayState : IGameState
    {
        public Task OnEnterAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnExitAsync()
        {
            return Task.CompletedTask;
        }
    }
}