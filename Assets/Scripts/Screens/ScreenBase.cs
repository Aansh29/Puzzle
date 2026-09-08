using System;
using UnityEngine;

namespace Puzzle.Screens
{
    public abstract class ScreenBase : MonoBehaviour
    {
        public abstract ScreenId ScreenId { get; }

        public abstract ScreenType ScreenType { get; }

        public virtual void Initialize(object payload)
        {
        }

        public virtual void Hide()
        {
            Destroy(gameObject);
        }
    }
}