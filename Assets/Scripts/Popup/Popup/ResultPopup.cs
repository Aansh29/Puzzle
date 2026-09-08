using Puzzle.Core;
using System;
using UnityEngine;

namespace Puzzle.Popups
{
    public sealed class ResultPopup : PopupBase
    {
        private Action<PopupResult> onClosed;

        public override PopupId PopupId => PopupId.Result;

        public override void Initialize(
            object payload,
            Action<PopupResult> onClosed)
        {
            base.Initialize(payload, onClosed);

            this.onClosed = onClosed;

            if (payload is not ResultPopupPayload data)
            {
                throw new ArgumentException($"Invalid payload for {nameof(ResultPopup)}.");
            }

            LevelResult result = data.Result;

            // TODO:
            // resultTitle.text = result.Outcome.ToString();
            // movesText.text = result.Moves.ToString();
            // scoreText.text = result.Score.ToString();
        }

        public void Close()
        {
            onClosed?.Invoke(PopupResult.Accepted);
        }
    }
}