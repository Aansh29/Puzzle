using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Gameplay.Grid
{
    public sealed class GridCellView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text valueText;

        public GridPosition Position { get; private set; }

        public void Initialize(GridPosition position)
        {
            Position = position;
        }

        public void SetValue(int value)
        {
            if (value == GridModel.EmptyCell)
            {
                valueText.text = string.Empty;
                return;
            }

            valueText.text = value.ToString();
        }
    }
}