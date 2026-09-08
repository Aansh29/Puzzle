using Puzzle.Gameplay.Grid;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Puzzle.Gameplay.Input
{
    public sealed class SwipeInputController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform inputArea;

        [SerializeField]
        private float minimumSwipeDistance = 50f;

        private Vector2 pointerStartPosition;

        private bool isPointerDown;

        public event Action<GridDirection> DirectionDetected;

        private void Awake()
        {
            Debug.Log("SwipeInputController Awake");
        }

        private void Update()
        {
            if (Pointer.current == null)
            {
                Debug.Log("Pointer.current is NULL");

                return;
            }

            if (Pointer.current.press.wasPressedThisFrame)
            {
                Vector2 pointerPosition =
                    Pointer.current.position.ReadValue();

                Debug.Log($"Pointer Down: {pointerPosition}");

                if (!IsInsideInputArea(pointerPosition))
                {
                    Debug.Log("Pointer Down ignored: Outside Input Area");

                    return;
                }

                pointerStartPosition = pointerPosition;

                isPointerDown = true;

                Debug.Log("Pointer Down accepted");
            }

            if (!isPointerDown)
            {
                return;
            }

            if (Pointer.current.press.wasReleasedThisFrame)
            {
                Vector2 pointerPosition =
                    Pointer.current.position.ReadValue();

                Debug.Log($"Pointer Up: {pointerPosition}");

                isPointerDown = false;

                DetectSwipe(pointerPosition);
            }
        }

        private bool IsInsideInputArea(Vector2 screenPosition)
        {
            if (inputArea == null)
            {
                return true;
            }

            Canvas canvas = inputArea.GetComponentInParent<Canvas>();

            if (canvas == null)
            {
                return true;
            }

            bool isInside =
                RectTransformUtility.RectangleContainsScreenPoint(
                    inputArea,
                    screenPosition,
                    canvas.worldCamera);

            return isInside;
        }

        private void DetectSwipe(Vector2 pointerEndPosition)
        {
            Vector2 delta =
                pointerEndPosition -
                pointerStartPosition;

            Debug.Log($"Swipe Delta: {delta}");

            if (delta.magnitude < minimumSwipeDistance)
            {
                Debug.Log(
                    $"Swipe rejected. Distance: {delta.magnitude}, " +
                    $"Required: {minimumSwipeDistance}");

                return;
            }

            GridDirection direction;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                direction = delta.x > 0
                    ? GridDirection.Right
                    : GridDirection.Left;
            }
            else
            {
                direction = delta.y > 0
                    ? GridDirection.Up
                    : GridDirection.Down;
            }

            Debug.Log($"Direction Detected: {direction}");

            DirectionDetected?.Invoke(direction);
        }
    }
}