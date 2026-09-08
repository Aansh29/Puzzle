using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Screens
{
    [CreateAssetMenu(menuName = "Puzzle/Screen Registry")]
    public class ScreenRegistry : ScriptableObject
    {
        [Serializable]
        private class ScreenEntry
        {
            public ScreenId Id;
            public ScreenBase Prefab;
        }

        [SerializeField]
        private List<ScreenEntry> screens = new();

        public ScreenBase GetPrefab(ScreenId screenId)
        {
            foreach (ScreenEntry screen in screens)
            {
                if (screen.Id == screenId)
                {
                    if (screen.Prefab == null)
                    {
                        throw new InvalidOperationException($"Screen prefab for '{screenId}' is null.");
                    }

                    return screen.Prefab;
                }
            }

            throw new InvalidOperationException( $"Screen '{screenId}' is not registered.");
        }
    }
}