using System;
using UnityEngine;

namespace Puzzle.Popups
{
    public abstract class PopupBase : MonoBehaviour
    {
        public abstract PopupId PopupId { get; }

        public virtual void Initialize(object payload, Action<PopupResult> onClosed)
        {
        }

        public virtual void Hide()
        {
            Destroy(gameObject);
        }
    }
}