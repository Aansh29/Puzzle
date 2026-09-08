using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Popups
{
    public sealed class PopupManager : IPopupManager
    {
        private readonly PopupRegistry popupRegistry;
        private readonly Transform popupRoot;

        private readonly Queue<PopupRequest> popupQueue = new();

        private PopupRequest activeRequest;
        private PopupBase activePopup;

        public PopupManager(PopupRegistry popupRegistry, Transform popupRoot)
        {
            this.popupRegistry = popupRegistry ?? throw new ArgumentNullException(nameof(popupRegistry));

            this.popupRoot = popupRoot ?? throw new ArgumentNullException(nameof(popupRoot));
        }

        public void Show(PopupId popupId, object payload, Func<bool> canShow, Action<PopupResult> onResult)
        {
            PopupRequest request = new PopupRequest(popupId, payload, canShow, onResult);

            if (activePopup == null)
            {
                ShowRequest(request);
                return;
            }

            popupQueue.Enqueue(request);
        }

        public void Hide(PopupId popupId)
        {
            if (activePopup == null || activeRequest == null || activePopup.PopupId != popupId)
            {
                return;
            }

            CloseActivePopup(PopupResult.Cancelled);
        }

        public void HideAll()
        {
            CancelQueuedRequests();

            if (activePopup != null)
            {
                CloseActivePopup(PopupResult.Cancelled);
            }
        }

        private void ShowRequest(PopupRequest request)
        {
            if (request.CanShow != null && !request.CanShow())
            {
                CompleteRequest(request, PopupResult.Cancelled);

                ShowNextPopup();
                return;
            }

            PopupBase prefab = popupRegistry.GetPrefab(request.PopupId);

            PopupBase instance = UnityEngine.Object.Instantiate(prefab, popupRoot);

            activeRequest = request;
            activePopup = instance;

            instance.Initialize(request.Payload, CloseActivePopup);
        }

        private void CloseActivePopup(PopupResult result)
        {
            if (activePopup == null ||
                activeRequest == null)
            {
                return;
            }

            PopupBase popupToDestroy = activePopup;

            activeRequest = null;
            activePopup = null;

            popupToDestroy.Hide(result);

            ShowNextPopup();
        }

        private void ShowNextPopup()
        {
            if (activePopup != null)
            {
                return;
            }

            while (popupQueue.Count > 0)
            {
                PopupRequest nextRequest = popupQueue.Dequeue();

                if (nextRequest.CanShow != null && !nextRequest.CanShow())
                {
                    CompleteRequest(nextRequest, PopupResult.Cancelled);
                    continue;
                }

                ShowRequest(nextRequest);
                return;
            }
        }

        private void CancelQueuedRequests()
        {
            while (popupQueue.Count > 0)
            {
                PopupRequest request = popupQueue.Dequeue();

                CompleteRequest(request, PopupResult.Cancelled);
            }
        }

        private void CompleteRequest(PopupRequest request, PopupResult result)
        {
            try
            {
                request.OnResult?.Invoke(result);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}