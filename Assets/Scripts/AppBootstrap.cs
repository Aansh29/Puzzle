using Puzzle.Core;
using Puzzle.Flow;
using Puzzle.Popups;
using Puzzle.Screens;
using Puzzle.Services;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.Bootstrap
{
    public sealed class AppBootstrap : MonoBehaviour
    {
        [Header("Registries")]
        [SerializeField] private ScreenRegistry screenRegistry;

        [SerializeField] private PopupRegistry popupRegistry;

        [Header("Roots")]
        [SerializeField] private Transform screenRoot;

        [SerializeField] private Transform popupRoot;

        [Header("Levels")]
        [SerializeField] private LevelData[] levels;

        private GameFlowController flowController;

        private async void Awake()
        {
            try
            {
                InitializeServices();

                await InitializeGameFlow();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void InitializeServices()
        {
            ServiceRegistry.Clear();

            ILevelService levelService = new LevelService(levels);

            IScreenManager screenManager = new ScreenManager(screenRegistry, screenRoot);

            IPopupManager popupManager = new PopupManager(popupRegistry, popupRoot);

            ISaveService saveService = new PlayerPrefsSaveService();

            if (!saveService.HasKey("CurrentLevel"))
            {
                saveService.SaveInt("CurrentLevel", 1);
            }

            ServiceRegistry.Register(saveService);

            ServiceRegistry.Register(levelService);
            ServiceRegistry.Register(screenManager);
            ServiceRegistry.Register(popupManager);
        }

        private async Task InitializeGameFlow()
        {
            IScreenManager screenManager = ServiceRegistry.Get<IScreenManager>();

            IPopupManager popupManager = ServiceRegistry.Get<IPopupManager>();

            MainMenuState mainMenuState = new MainMenuState(screenManager);

            GameplayState gameplayState = new GameplayState(screenManager);

            ResultsState resultsState = new ResultsState(popupManager);

            flowController = new GameFlowController(mainMenuState, gameplayState, resultsState);

            ServiceRegistry.Register<IGameFlowController>(flowController);

            await flowController.InitializeAsync();
        }

        private void OnDestroy()
        {
            ServiceRegistry.Clear();
        }
    }
}