using System.Runtime.CompilerServices;

namespace BlockDrawBlast.Gameplay
{
    public struct DrawingBlockContext
    {
        public MatrixPosition position;
        public ColorType colorType;
        public int orderIndex;
        
        public BlockData ToBlockData()
        {
            return new BlockData
            {
                colorType = colorType,
                blockType = BlockType.Normal,
                position = position,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid(int rows, int columns)
        {
            return position.IsValid(rows, columns);
        }
    }
}

