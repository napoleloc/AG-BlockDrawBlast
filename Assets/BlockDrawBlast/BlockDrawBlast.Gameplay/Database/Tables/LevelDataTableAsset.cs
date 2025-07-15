using System;
using System.Runtime.CompilerServices;
using EncosyTower.Common;
using UnityEngine;

namespace BlockDrawBlast.Database
{
    public class LevelDataTableAsset : ScriptableObject
    {
        [SerializeField] private LevelDataAsset[] _levels;

        public ReadOnlyMemory<LevelDataAsset> Levels
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _levels;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<LevelDataAsset> GetLevelData(int level)
            => TryGetLevelData(level, out var levelData) ? levelData : default;
        
        public bool TryGetLevelData(int level, out LevelDataAsset levelData)
        {
            if (level < 0 || level >= _levels.Length)
            {
                levelData = default;
                return false;
            }

            levelData = Levels.Span[level];
            return true;
        }
    }
}

