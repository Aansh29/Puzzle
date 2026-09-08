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
        public int Score { get; }

        public LevelResult(LevelOutcome outcome, int moves, int score)
        {
            Outcome = outcome;
            Moves = moves;
            Score = score;
        }
    }
}