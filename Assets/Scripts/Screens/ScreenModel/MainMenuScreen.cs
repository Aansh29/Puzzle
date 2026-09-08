using Puzzle.Core;
using Puzzle.Flow;
using Puzzle.Services;
using TMPro;
using UnityEngine;

namespace Puzzle.Screens
{
    public sealed class MainMenuScreen : ScreenBase
    {

        [SerializeField] private TMP_Text levelText;
        public override ScreenId ScreenId => ScreenId.MainMenu;

        public override ScreenType ScreenType => ScreenType.Exclusive;

        public override void Initialize(object payload)
        {
            ISaveService saveService = ServiceRegistry.Get<ISaveService>();

            levelText.text = "<color=#2196F3> Level </color>" + $"<color=#F44336>{saveService.LoadInt("CurrentLevel")}</color>";
        }

        public void OnPlayClicked()
        {
            ISaveService saveService = ServiceRegistry.Get<ISaveService>();

            ILevelService levelService = ServiceRegistry.Get<ILevelService>();

            IGameFlowController flowController = ServiceRegistry.Get<IGameFlowController>();

            int currentLevel = saveService.LoadInt("CurrentLevel");

            LevelData levelData = levelService.GetLevel(currentLevel);

            _ = flowController.StartLevelAsync(levelData);
        }
    }
}