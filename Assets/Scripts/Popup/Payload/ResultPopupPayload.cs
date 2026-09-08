using Puzzle.Core;

namespace Puzzle.Popups
{
    public readonly struct ResultPopupPayload
    {
        public LevelResult Result { get; }

        public ResultPopupPayload(LevelResult result)
        {
            Result = result;
        }
    }
}