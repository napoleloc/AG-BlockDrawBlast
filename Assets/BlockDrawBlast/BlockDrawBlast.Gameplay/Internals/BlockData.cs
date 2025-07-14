using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly record struct BlockData
    {
        public int Row { get; init; }
        public int Column { get; init; }
        public ColorType ColorType { get; init; }
        public BlockType BlockType { get; init; }
        public int RequiredKeyCount { get; init; }
        public int Health { get; init; } 
        
        public float3 WorldPosition { get; init; }

        public static BlockData CreateNormal(int row, int column, ColorType colorType, float3 worldPosition)
        {
            return new BlockData
            {
                Row = row,
                Column = column,
                ColorType = colorType,
                BlockType = BlockType.Normal,
                WorldPosition = worldPosition,
            };
        }
        
        public static BlockData CreateKey(int row, int column, ColorType colorType, float3 worldPosition)
        {
            return new BlockData
            {
                Row = row,
                Column = column,
                ColorType = colorType,
                BlockType = BlockType.Key,
                WorldPosition = worldPosition,
            };
        }
        
        public static BlockData CreateLocked(int row, int column, ColorType colorType, int requiredKeyCount, float3 worldPosition)
        {
            return new BlockData
            {
                Row = row,
                Column = column,
                ColorType = colorType,
                BlockType = BlockType.Locked,
                RequiredKeyCount = requiredKeyCount,
                WorldPosition = worldPosition,
            };
        }
        
        public static BlockData CreateMultiHit(int row, int column, ColorType colorType, int health, float3 worldPosition)
        {
            return new BlockData
            {
                Row = row,
                Column = column,
                ColorType = colorType,
                BlockType = BlockType.MultiHit,
                WorldPosition = worldPosition,
                Health = health
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAdjacentTo(BlockData other)
        {
            var rowDiff = math.abs(Row - other.Row);
            var colDiff = math.abs(Column - other.Column);
            return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDiagonalTo(BlockData other)
        {
            var rowDiff = math.abs(Row - other.Row);
            var colDiff = math.abs(Column - other.Column);
            return rowDiff == 1 && colDiff == 1;
        }

    }
}
