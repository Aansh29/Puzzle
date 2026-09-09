using Puzzle.Gameplay.Grid;
using System;
using System.Collections.Generic;

namespace Puzzle.Gameplay.Hints
{
    public sealed class HintService : IHintService
    {
        private sealed class SearchNode
        {
            public int[] Cells { get; }

            public int Cost { get; }

            public int Heuristic { get; }

            public SearchNode Parent { get; }

            public GridPosition SourcePosition { get; }

            public GridDirection Direction { get; }

            public int Score => Cost + Heuristic;

            public SearchNode(int[] cells, int cost, int heuristic, SearchNode parent, GridPosition sourcePosition, GridDirection direction)
            {
                Cells = cells;
                Cost = cost;
                Heuristic = heuristic;
                Parent = parent;
                SourcePosition = sourcePosition;
                Direction = direction;
            }
        }

        private sealed class PriorityQueue<T>
        {
            private readonly List<QueueNode> nodes = new();

            private long order;

            public int Count => nodes.Count;

            public void Enqueue(T item, int priority)
            {
                QueueNode node = new QueueNode(item, priority, order++);
                nodes.Add(node);
                BubbleUp(nodes.Count - 1);
            }

            public T Dequeue()
            {
                if (nodes.Count == 0)
                {
                    throw new InvalidOperationException("Priority queue is empty.");
                }

                T result = nodes[0].Item;
                int lastIndex = nodes.Count - 1;
                nodes[0] = nodes[lastIndex];
                nodes.RemoveAt(lastIndex);

                if (nodes.Count > 0)
                {
                    BubbleDown(0);
                }

                return result;
            }

            private void BubbleUp(int index)
            {
                while (index > 0)
                {
                    int parentIndex = (index - 1) / 2;

                    if (!IsHigherPriority(nodes[index], nodes[parentIndex]))
                    {
                        break;
                    }

                    Swap(index, parentIndex);
                    index = parentIndex;
                }
            }

            private void BubbleDown(int index)
            {
                while (true)
                {
                    int leftIndex = index * 2 + 1;
                    int rightIndex = index * 2 + 2;
                    int highestIndex = index;

                    if (leftIndex < nodes.Count && IsHigherPriority(nodes[leftIndex], nodes[highestIndex]))
                    {
                        highestIndex = leftIndex;
                    }

                    if (rightIndex < nodes.Count && IsHigherPriority(nodes[rightIndex], nodes[highestIndex]))
                    {
                        highestIndex = rightIndex;
                    }

                    if (highestIndex == index)
                    {
                        break;
                    }

                    Swap(index, highestIndex);
                    index = highestIndex;
                }
            }

            private bool IsHigherPriority(QueueNode first, QueueNode second)
            {
                if (first.Priority != second.Priority)
                {
                    return first.Priority < second.Priority;
                }

                return first.Order < second.Order;
            }

            private void Swap(int firstIndex, int secondIndex)
            {
                QueueNode temporary = nodes[firstIndex];
                nodes[firstIndex] = nodes[secondIndex];
                nodes[secondIndex] = temporary;
            }

            private sealed class QueueNode
            {
                public T Item { get; }

                public int Priority { get; }

                public long Order { get; }

                public QueueNode(T item, int priority, long order)
                {
                    Item = item;
                    Priority = priority;
                    Order = order;
                }
            }
        }

        private static readonly GridDirection[] Directions =
        {
            GridDirection.Up,
            GridDirection.Down,
            GridDirection.Left,
            GridDirection.Right
        };

        public bool TryGetHint(GridModel gridModel, out GridPosition sourcePosition, out GridDirection direction)
        {
            sourcePosition = default;
            direction = default;

            if (gridModel == null)
            {
                throw new ArgumentNullException(nameof(gridModel));
            }

            if (gridModel.IsSolved())
            {
                return false;
            }

            int[] initialCells = CreateState(gridModel);
            SearchNode solution = FindSolution(initialCells, gridModel.Rows, gridModel.Columns);

            if (solution == null)
            {
                return false;
            }

            SearchNode firstMove = solution;

            while (firstMove.Parent != null && firstMove.Parent.Parent != null)
            {
                firstMove = firstMove.Parent;
            }

            sourcePosition = firstMove.SourcePosition;
            direction = firstMove.Direction;

            return true;
        }

        private SearchNode FindSolution(int[] initialCells, int rows, int columns)
        {
            int initialHeuristic = CalculateManhattanDistance(initialCells, rows, columns);
            SearchNode startNode = new SearchNode(initialCells, 0, initialHeuristic, null, default, default);
            PriorityQueue<SearchNode> openSet = new PriorityQueue<SearchNode>();
            Dictionary<string, int> visitedCosts = new Dictionary<string, int>();
            string startKey = CreateKey(initialCells);

            visitedCosts[startKey] = 0;

            openSet.Enqueue(startNode, startNode.Score);

            while (openSet.Count > 0)
            {
                SearchNode current = openSet.Dequeue();

                if (IsSolved(current.Cells, rows, columns))
                {
                    return current;
                }

                string currentKey = CreateKey(current.Cells);

                if (visitedCosts.TryGetValue(currentKey, out int knownCost) && current.Cost > knownCost)
                {
                    continue;
                }

                for (int emptyIndex = 0; emptyIndex < current.Cells.Length; emptyIndex++)
                {
                    if (current.Cells[emptyIndex] != GridModel.EmptyCell)
                    {
                        continue;
                    }

                    GridPosition emptyPosition = new GridPosition(emptyIndex / columns, emptyIndex % columns);

                    foreach (GridDirection candidateDirection in Directions)
                    {
                        if (!TryGetSourcePosition(emptyPosition, candidateDirection, rows, columns, out GridPosition sourcePosition))
                        {
                            continue;
                        }

                        int sourceIndex = sourcePosition.Row * columns + sourcePosition.Column;
                        int[] nextCells = (int[])current.Cells.Clone();

                        nextCells[emptyIndex] = nextCells[sourceIndex];
                        nextCells[sourceIndex] = GridModel.EmptyCell;

                        int nextCost = current.Cost + 1;
                        string nextKey = CreateKey(nextCells);

                        if (visitedCosts.TryGetValue(nextKey, out int previousCost) && previousCost <= nextCost)
                        {
                            continue;
                        }

                        int heuristic = CalculateManhattanDistance(nextCells, rows, columns);
                        SearchNode nextNode = new SearchNode(nextCells, nextCost, heuristic, current, sourcePosition, candidateDirection);

                        visitedCosts[nextKey] = nextCost;
                        openSet.Enqueue(nextNode, nextNode.Score);
                    }
                }
            }

            return null;
        }

        private int[] CreateState(GridModel gridModel)
        {
            int[] cells = new int[gridModel.Rows * gridModel.Columns];

            for (int row = 0; row < gridModel.Rows; row++)
            {
                for (int column = 0; column < gridModel.Columns; column++)
                {
                    GridPosition position = new GridPosition(row, column);
                    cells[row * gridModel.Columns + column] = gridModel.GetCell(position);
                }
            }

            return cells;
        }

        private bool TryGetSourcePosition(GridPosition emptyPosition, GridDirection direction, int rows, int columns, out GridPosition sourcePosition)
        {
            sourcePosition = default;

            int row = emptyPosition.Row;
            int column = emptyPosition.Column;

            switch (direction)
            {
                case GridDirection.Up:
                    row++;
                    break;

                case GridDirection.Down:
                    row--;
                    break;

                case GridDirection.Left:
                    column++;
                    break;

                case GridDirection.Right:
                    column--;
                    break;

                default:
                    return false;
            }

            if (row < 0 || row >= rows || column < 0 || column >= columns)
            {
                return false;
            }

            sourcePosition = new GridPosition(row, column);

            return true;
        }

        private bool IsSolved(int[] cells, int rows, int columns)
        {
            int expectedValue = 1;

            for (int index = 0; index < cells.Length; index++)
            {
                if (index >= cells.Length - CountEmptySpaces(cells))
                {
                    return cells[index] == GridModel.EmptyCell;
                }

                if (cells[index] != expectedValue)
                {
                    return false;
                }

                expectedValue++;
            }

            return true;
        }

        private int CountEmptySpaces(int[] cells)
        {
            int emptySpaces = 0;

            for (int index = 0; index < cells.Length; index++)
            {
                if (cells[index] == GridModel.EmptyCell)
                {
                    emptySpaces++;
                }
            }

            return emptySpaces;
        }

        private int CalculateManhattanDistance(int[] cells, int rows, int columns)
        {
            int totalDistance = 0;

            for (int index = 0; index < cells.Length; index++)
            {
                int value = cells[index];

                if (value == GridModel.EmptyCell)
                {
                    continue;
                }

                int currentRow = index / columns;
                int currentColumn = index % columns;
                int targetIndex = value - 1;
                int targetRow = targetIndex / columns;
                int targetColumn = targetIndex % columns;

                totalDistance += Math.Abs(currentRow - targetRow);
                totalDistance += Math.Abs(currentColumn - targetColumn);
            }

            return totalDistance;
        }

        private string CreateKey(int[] cells)
        {
            return string.Join(",", cells);
        }
    }
}