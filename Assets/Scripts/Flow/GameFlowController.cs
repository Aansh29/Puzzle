using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Puzzle.Core;

namespace Puzzle.Flow
{
    public sealed class GameFlowController : IGameFlowController
    {
        private readonly Dictionary<GameStateId, IGameState> states;

        private IGameState activeState;

        public GameStateId CurrentState { get; private set; }

        public LevelContext CurrentLevelContext { get; private set; }

        public LevelResult CurrentLevelResult { get; private set; }

        public GameFlowController(IGameState mainMenuState, GameplayState gameplayState, ResultsState resultsState)
        {
            states = new Dictionary<GameStateId, IGameState>
            {
                { GameStateId.MainMenu, mainMenuState },
                { GameStateId.Gameplay, gameplayState },
                { GameStateId.Results, resultsState }
            };

            gameplayState.SetFlowController(this);
            resultsState.SetFlowController(this);
        }

        public async Task InitializeAsync()
        {
            await ChangeStateAsync(GameStateId.MainMenu);
        }

        public async Task ChangeStateAsync(GameStateId stateId)
        {
            if (!states.TryGetValue(stateId, out IGameState nextState))
            {
                throw new InvalidOperationException($"Game state '{stateId}' is not registered.");
            }

            if (activeState != null)
            {
                await activeState.OnExitAsync();
            }

            activeState = nextState;
            CurrentState = stateId;

            await activeState.OnEnterAsync();
        }

        public async Task StartLevelAsync(LevelData levelData)
        {
            if (levelData == null)
            {
                throw new ArgumentNullException(nameof(levelData));
            }

            CurrentLevelContext = new LevelContext(levelData);
            CurrentLevelResult = null;

            await ChangeStateAsync(GameStateId.Gameplay);
        }

        public async Task CompleteLevelAsync(LevelResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            CurrentLevelResult = result;

            await ChangeStateAsync(GameStateId.Results);
        }

        public async Task QuitToMainMenuAsync()
        {
            CurrentLevelContext = null;
            CurrentLevelResult = null;

            await ChangeStateAsync(GameStateId.MainMenu);
        }
    }
}