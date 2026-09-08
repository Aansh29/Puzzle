namespace Puzzle.Core
{
    public enum LevelOutcome
    {
        Win,
        Lose,
        Quit
    }

    public sealed class LevelResult
    {
        public LevelOutcome Outcome { get; }
        public int Moves { get; }

        public LevelResult(LevelOutcome outcome, int moves)
        {
            Outcome = outcome;
            Moves = moves;
        }
    }
}