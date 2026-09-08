using Puzzle.Core;
using Puzzle.Flow;
using Puzzle.Services;
using System;
using TMPro;
using UnityEngine;

namespace Puzzle.Popups
{
    public sealed class ResultPopup : PopupBase
    {
        [SerializeField]
        private TMP_Text resultText;

        [SerializeField]
        private GameObject continueButton;

        [SerializeField]
        private GameObject restartButton;

        public override PopupId PopupId => PopupId.Result;

        public override void Initialize(object payload, Action<PopupResult> onClosed)
        {
            base.Initialize(payload, onClosed);

            if (payload is not ResultPopupPayload data)
            {
                throw new ArgumentException($"Invalid payload for {nameof(ResultPopup)}.");
            }

            LevelResult result = data.Result;

            resultText.text = result.Outcome == LevelOutcome.Win ? "YOU WIN" : "GAME OVER";

            SetupButtons(result.Outcome);
        }

        public void OnContinueClicked()
        {
            IGameFlowController flowController = ServiceRegistry.Get<IGameFlowController>();

            Hide();

            _ = flowController.QuitToMainMenuAsync();
        }

        public void OnRestartClicked()
        {
            IGameFlowController flowController = ServiceRegistry.Get<IGameFlowController>();

            LevelData levelData = flowController.CurrentLevelContext.LevelData;

            Hide();

            _ = flowController.StartLevelAsync(levelData);
        }

        private void SetupButtons(LevelOutcome outcome)
        {
            continueButton.SetActive(true);
            restartButton.SetActive(outcome == LevelOutcome.Lose);
        }
    }
}