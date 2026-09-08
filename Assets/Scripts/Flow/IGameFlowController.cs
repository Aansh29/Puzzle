using System.Threading.Tasks;
using Puzzle.Core;

namespace Puzzle.Flow
{
    public interface IGameFlowController
    {
        GameStateId CurrentState { get; }

        LevelContext CurrentLevelContext { get; }
        LevelResult CurrentLevelResult { get; }

        Task InitializeAsync();
        Task ChangeStateAsync(GameStateId stateId);

        Task StartLevelAsync(LevelData levelData);
        Task CompleteLevelAsync(LevelResult result);
        Task QuitToMainMenuAsync();
    }
}