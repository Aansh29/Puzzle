using Coffee.UIExtensions;
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

        [SerializeField]
        private GameObject Container;

        [SerializeField] private CanvasGroup ButtonParent;

        [SerializeField]
        private Transform MoveLocation;

        [SerializeField] private UIParticle confettiVfx;

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
            ShowResultAnimation(result.Outcome);
        }

        public void ShowResultAnimation(LevelOutcome outcome)
        {
            CanvasGroup group1 = resultText.GetComponent<CanvasGroup>();

            group1.alpha = 0f;
            ButtonParent.alpha = 0f;

            LeanTween.move(Container, MoveLocation, 0.7f)
                .setEaseOutCubic()
                .setOnComplete(() =>
                {
                    LeanTween.alphaCanvas(group1, 1f, 0.3f);

                    LeanTween.alphaCanvas(ButtonParent, 1f, 0.3f)
                        .setOnComplete(() =>
                        {
                            if(outcome == LevelOutcome.Win)
                            {
                                confettiVfx.Play();
                            }
                        });
                });
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