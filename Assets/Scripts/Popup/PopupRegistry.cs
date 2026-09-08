using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Popups
{
    [CreateAssetMenu(menuName = "Puzzle/Popup Registry")]
    public class PopupRegistry : ScriptableObject
    {
        [Serializable]
        private class PopupEntry
        {
            public PopupId Id;
            public PopupBase Prefab;
        }

        [SerializeField]
        private List<PopupEntry> popups = new();

        public PopupBase GetPrefab(PopupId popupId)
        {
            foreach (PopupEntry popup in popups)
            {
                if (popup.Id == popupId)
                {
                    if (popup.Prefab == null)
                    {
                        throw new InvalidOperationException($"Popup prefab for '{popupId}' is null.");
                    }

                    return popup.Prefab;
                }
            }

            throw new InvalidOperationException($"Popup '{popupId}' is not registered.");
        }
    }
}