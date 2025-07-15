using System;
using System.Runtime.CompilerServices;
using BlockDrawBlast.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlockDrawBlast.Database
{
    [CreateAssetMenu(fileName = "level-data-asset-", menuName = "BlockDrawBlast/Database/Level Data Asset")]
    public class LevelDataAsset : ScriptableObject
    {
        [PropertyRange(1, 10)]
        public int rows = 1;
        [PropertyRange(1, 10)]
        public int columns = 1;

        [Title("Dependencies", titleAlignment: TitleAlignments.Centered)]
        public Transform source;
        
        [Title("Arrays", titleAlignment: TitleAlignments.Centered)]
        public LevelBlockData[] preparedLevelBlockDataArray;
        public LevelTileData[] preparedLevelTileDataArray;
        public LevelStackBlockData[] preparedLevelStackBlockDataArray;

        public ReadOnlyMemory<LevelBlockData> PreparedLevelBlocks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => preparedLevelBlockDataArray;
        }

        public ReadOnlyMemory<LevelTileData> PreparedLevelTiles
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => preparedLevelTileDataArray;
        }
        
        public ReadOnlyMemory<LevelStackBlockData> PreparedLevelStackBlocks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => preparedLevelStackBlockDataArray;
        }
    }

    [Serializable]
    public struct LevelBlockData
    {
        public int row;
        public int column;
        
        public BlockType blockType;
        public ColorType colorType;
        
        public int requiredKeys;
    }

    [Serializable]
    public struct LevelTileData
    {
        public int row;
        public int column;
        
        public TileFlag flag;
    }

    [Serializable]
    public struct LevelStackBlockData
    {
        public ColorType colorType;
        public int count;
    }
}

