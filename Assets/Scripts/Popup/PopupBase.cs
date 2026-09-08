using System;
using UnityEngine;

namespace Puzzle.Popups
{
    public abstract class PopupBase : MonoBehaviour
    {
        public abstract PopupId PopupId { get; }

        private Action<PopupResult> onClosed;

        public virtual void Initialize(object payload, Action<PopupResult> onClosed)
        {
            this.onClosed = onClosed;
        }

        public virtual void Hide()
        {
            Hide(PopupResult.Accepted);
        }

        public virtual void Hide(PopupResult result)
        {
            onClosed?.Invoke(result);

            Destroy(gameObject);
        }
    }
}