using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Puzzle.Gameplay.Grid
{
    public sealed class GridView : MonoBehaviour
    {
        [SerializeField]
        private GridCellView cellPrefab;

        [SerializeField]
        private float cellSize = 100f;

        [SerializeField]
        private float spacing = 10f;

        [SerializeField]
        private float moveDuration = 0.15f;

        private readonly Dictionary<int, GridCellView> cells = new();

        [Button]
        private void DrawBoxes()
        {
            GridModel gridModel = new GridModel(5, 7);

            Build(gridModel);
        }

        public void Build(GridModel gridModel)
        {
            if (gridModel == null)
            {
                throw new ArgumentNullException(nameof(gridModel));
            }

            Clear();

            for (int row = 0; row < gridModel.Rows; row++)
            {
                for (int column = 0; column < gridModel.Columns; column++)
                {
                    GridPosition position = new GridPosition(row, column);

                    int value = gridModel.GetCell(position);

                    if (value == GridModel.EmptyCell)
                    {
                        continue;
                    }

                    GridCellView cell = Instantiate(cellPrefab, transform);

                    cell.Initialize(value);

                    cell.transform.localPosition = CalculatePosition(position, gridModel.Rows, gridModel.Columns);

                    cells.Add(value, cell);
                }
            }
        }

        public void MoveTile(int value, GridPosition targetPosition, int rows, int columns)
        {
            if (!cells.TryGetValue(value, out GridCellView cell))
            {
                return;
            }

            Vector2 target = CalculatePosition(targetPosition, rows, columns);

            LeanTween.cancel(cell.gameObject);

            LeanTween.moveLocal(
                    cell.gameObject,
                    target,
                    moveDuration)
                .setEaseOutQuad();
        }

        public void Clear()
        {
            foreach (GridCellView cell in cells.Values)
            {
                if (cell != null)
                {
                    Destroy(cell.gameObject);
                }
            }

            cells.Clear();
        }

        private Vector2 CalculatePosition(GridPosition position, int rows, int columns)
        {
            float totalWidth = columns * cellSize + (columns - 1) * spacing;

            float totalHeight = rows * cellSize + (rows - 1) * spacing;

            float startX =- (totalWidth - cellSize) * 0.5f;

            float startY = (totalHeight - cellSize) * 0.5f;

            float x = startX + position.Column * (cellSize + spacing);

            float y = startY - position.Row * (cellSize + spacing);

            return new Vector2(x, y);
        }
    }
}