using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Puzzle.Screens
{
    public sealed class ScreenManager : IScreenManager
    {
        private readonly ScreenRegistry screenRegistry;
        private readonly Transform screenRoot;

        private readonly List<ScreenBase> additiveScreens = new();

        private ScreenBase activeScreen;
        private ScreenBase hiddenScreen;

        public ScreenManager(ScreenRegistry screenRegistry, Transform screenRoot)
        {
            this.screenRegistry = screenRegistry ?? throw new ArgumentNullException(nameof(screenRegistry));

            this.screenRoot = screenRoot ?? throw new ArgumentNullException(nameof(screenRoot));
        }

        public Task ShowAsync(ScreenId screenId, object payload = null)
        {
            ScreenBase prefab = screenRegistry.GetPrefab(screenId);

            switch (prefab.ScreenType)
            {
                case ScreenType.Exclusive:
                    ShowExclusive(screenId, payload);
                    break;

                case ScreenType.Modal:
                    ShowModal(screenId, payload);
                    break;

                case ScreenType.Additive:
                    ShowAdditive(screenId, payload);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }

        public Task HideAsync(ScreenId screenId)
        {
            if (activeScreen != null && activeScreen.ScreenId == screenId)
            {
                HideActiveScreen();

                if (hiddenScreen != null)
                {
                    RestoreHiddenScreen();
                }

                return Task.CompletedTask;
            }

            if (hiddenScreen != null && hiddenScreen.ScreenId == screenId)
            {
                DestroyHiddenScreen();

                return Task.CompletedTask;
            }

            for (int i = additiveScreens.Count - 1; i >= 0; i--)
            {
                ScreenBase screen = additiveScreens[i];

                if (screen.ScreenId != screenId)
                {
                    continue;
                }

                screen.Hide();
                additiveScreens.RemoveAt(i);
            }

            return Task.CompletedTask;
        }

        public Task HideAllAsync()
        {
            DestroyActiveScreen();
            DestroyHiddenScreen();
            DestroyAdditiveScreens();

            return Task.CompletedTask;
        }

        private void ShowExclusive(ScreenId screenId, object payload)
        {
            // Exclusive replaces everything.
            DestroyActiveScreen();
            DestroyHiddenScreen();
            DestroyAdditiveScreens();

            ShowScreen(screenId, payload);
        }

        private void ShowModal(ScreenId screenId, object payload)
        {
            // Additive -> Modal is rejected.
            if (activeScreen == null && additiveScreens.Count > 0)
            {
                throw new InvalidOperationException("A Modal screen cannot be pushed over an Additive screen.");
            }

            // None -> Modal.
            if (activeScreen == null)
            {
                ShowScreen(screenId, payload);

                return;
            }

            // Exclusive -> Modal.
            // Hide Exclusive and keep it for restoration.
            if (activeScreen.ScreenType == ScreenType.Exclusive)
            {
                hiddenScreen = activeScreen;
                activeScreen = null;

                hiddenScreen.gameObject.SetActive(false);
            }
            // Modal -> Modal.
            // Destroy previous Modal.
            else if (activeScreen.ScreenType == ScreenType.Modal)
            {
                DestroyActiveScreen();
            }

            ShowScreen(screenId, payload);
        }

        private void ShowAdditive(ScreenId screenId, object payload)
        {
            // Exclusive -> Additive is rejected.
            if (activeScreen != null && activeScreen.ScreenType == ScreenType.Exclusive)
            {
                throw new InvalidOperationException("An Additive screen cannot be pushed over an Exclusive screen.");
            }

            // None -> Additive.
            // Modal -> Additive.
            // Additive -> Additive.
            ScreenBase screen = CreateScreen(screenId, payload);

            additiveScreens.Add(screen);
        }

        private void RestoreHiddenScreen()
        {
            if (hiddenScreen == null)
            {
                return;
            }

            activeScreen = hiddenScreen;
            hiddenScreen = null;

            activeScreen.gameObject.SetActive(true);
        }

        private void DestroyActiveScreen()
        {
            if (activeScreen == null)
            {
                return;
            }

            ScreenBase screenToDestroy = activeScreen;

            activeScreen = null;

            screenToDestroy.Hide();
        }

        private void DestroyHiddenScreen()
        {
            if (hiddenScreen == null)
            {
                return;
            }

            ScreenBase screenToDestroy = hiddenScreen;

            hiddenScreen = null;

            screenToDestroy.Hide();
        }

        private void DestroyAdditiveScreens()
        {
            for (int i = additiveScreens.Count - 1; i >= 0; i--)
            {
                additiveScreens[i].Hide();
            }

            additiveScreens.Clear();
        }

        private void HideActiveScreen()
        {
            if (activeScreen == null)
            {
                return;
            }

            ScreenBase screenToHide = activeScreen;

            activeScreen = null;

            screenToHide.Hide();
        }

        private void ShowScreen(ScreenId screenId, object payload)
        {
            activeScreen = CreateScreen(screenId, payload);
        }

        private ScreenBase CreateScreen(ScreenId screenId, object payload)
        {
            ScreenBase prefab = screenRegistry.GetPrefab(screenId);

            ScreenBase instance = UnityEngine.Object.Instantiate(prefab, screenRoot);

            instance.Initialize(payload);

            return instance;
        }
    }
}