using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle.Gameplay.Grid
{
    public sealed class GridCellView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private TMP_Text valueText;

        public int Value { get; private set; }
        public GridPosition Position { get; private set; }

        private Vector2 pointerDownPosition;

        public System.Action<GridPosition, Vector2> SwipeDetected;

        public void Initialize(int value, GridPosition position)
        {
            Value = value;
            Position = position;

            SetValue(value);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDownPosition = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Vector2 swipeDelta = eventData.position - pointerDownPosition;

            if (swipeDelta.magnitude < 50f)
            {
                return;
            }

            SwipeDetected?.Invoke(Position, swipeDelta);
        }

        public void SetValue(int value)
        {
            Value = value;

            if (value == GridModel.EmptyCell)
            {
                valueText.text = string.Empty;
                return;
            }

            valueText.text = value.ToString();
        }
        public void SetPosition(GridPosition position)
        {
            Position = position;
        }
    }
}