using UnityEngine;

namespace Puzzle.Core
{
    [CreateAssetMenu(menuName = "Puzzle/Level Data")]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public string LevelName { get; private set; }
        [field: SerializeField] public int LevelNumber { get; private set; }
        [field: SerializeField] public int Rows { get; private set; }
        [field: SerializeField] public int Columns { get; private set; }
        [field: SerializeField] public int MoveLimit { get; private set; }
    }
}