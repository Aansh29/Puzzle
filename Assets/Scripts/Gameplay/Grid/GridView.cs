using System;
using UnityEngine;
using NaughtyAttributes;

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

        private GridCellView[,] cells;

        [Button]
        private void DrawBoxes()
        {
            GridModel gridModel =
                new GridModel(5, 7);

            Build(gridModel);
        }

        public void Build(GridModel gridModel)
        {
            if (gridModel == null)
            {
                throw new ArgumentNullException(nameof(gridModel));
            }

            Clear();

            cells = new GridCellView[gridModel.Rows, gridModel.Columns];

            for (int row = 0; row < gridModel.Rows; row++)
            {
                for (int column = 0; column < gridModel.Columns; column++)
                {
                    GridPosition position = new GridPosition(row, column);

                    GridCellView cell = Instantiate(cellPrefab, transform);

                    cell.transform.localPosition = CalculatePosition(position, gridModel.Rows, gridModel.Columns);

                    cell.Initialize(position);

                    cells[row, column] = cell;
                }
            }

            Render(gridModel);
        }

        public void Render(GridModel gridModel)
        {
            if (gridModel == null)
            {
                throw new ArgumentNullException(nameof(gridModel));
            }

            for (int row = 0; row < gridModel.Rows; row++)
            {
                for (int column = 0; column < gridModel.Columns; column++)
                {
                    GridPosition position = new GridPosition(row, column);

                    cells[row, column].SetValue(gridModel.GetCell(position));
                }
            }
        }

        public void Clear()
        {
            if (cells == null)
            {
                return;
            }

            for (int row = 0; row < cells.GetLength(0); row++)
            {
                for (int column = 0; column < cells.GetLength(1); column++)
                {
                    if (cells[row, column] != null)
                    {
                        Destroy(cells[row, column].gameObject);
                    }
                }
            }

            cells = null;
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