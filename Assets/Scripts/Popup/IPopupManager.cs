using System;

namespace Puzzle.Popups
{
    public interface IPopupManager
    {
        void Show(PopupId popupId, object payload, Func<bool> canShow, Action<PopupResult> onResult);

        void Hide(PopupId popupId);

        void HideAll();
    }
}