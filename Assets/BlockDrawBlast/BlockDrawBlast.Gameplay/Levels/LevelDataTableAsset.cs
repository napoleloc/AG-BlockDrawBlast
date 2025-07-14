using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BlockDrawBlast.Gameplay.Levels
{
    public class LevelDataTableAsset : ScriptableObject
    {
        [SerializeField] private LevelData[] _entries;

        private readonly Dictionary<int, LevelData> _idToLevelData = new();
        
        public ReadOnlyMemory<LevelData> Entries
        {
            [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
            get => _entries;
        }

        public void Initialize()
        {
            var entries = Entries.Span;
            var length = entries.Length;
            var idToLevelData = _idToLevelData;
            
            idToLevelData.Clear();
            idToLevelData.EnsureCapacity(length);
            
            for (var i = 0; i < length; i++)
            {
                var entry = entries[i];
                idToLevelData.Add(entry.rows, entry);
            }
        }
        
        public void Deinitialize()
        {
            _idToLevelData.Clear();
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public bool TryGetLevelData(int level, out LevelData entry)
        {
            var result = _idToLevelData.TryGetValue(level, out entry);
            return result;
        }
    }
}

