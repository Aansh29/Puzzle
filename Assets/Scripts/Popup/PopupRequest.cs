using System;

namespace Puzzle.Popups
{
    internal sealed class PopupRequest
    {
        public PopupId PopupId { get; }
        public object Payload { get; }
        public Func<bool> CanShow { get; }
        public Action<PopupResult> OnResult { get; }

        public PopupRequest(PopupId popupId, object payload, Func<bool> canShow, Action<PopupResult> onResult)
        {
            PopupId = popupId;
            Payload = payload;
            CanShow = canShow;
            OnResult = onResult;
        }
    }
}