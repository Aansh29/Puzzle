using TMPro;
using UnityEngine;

namespace Puzzle.Gameplay.Grid
{
    public sealed class GridCellView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text valueText;

        public int Value { get; private set; }

        public void Initialize(int value)
        {
            Value = value;

            SetValue(value);
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
    }
}