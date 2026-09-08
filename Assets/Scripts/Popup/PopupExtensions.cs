using System;
using Puzzle.Core;

namespace Puzzle.Popups
{
    public static class PopupExtensions
    {
        public static void ShowResultPopup(this IPopupManager popupManager, LevelResult levelResult, Func<bool> canShow, Action<PopupResult> onResult)
        {
            ResultPopupPayload payload = new ResultPopupPayload(levelResult);

            popupManager.Show(PopupId.Result, payload, canShow, onResult);
        }
    }
}