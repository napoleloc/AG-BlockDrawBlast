
namespace BlockDrawBlast.Gameplay
{
    public struct DrawingBlockContext
    {
        public MatrixPosition position;
        public ColorType colorType;
        
        public BlockData ToBlockData()
        {
            return new BlockData
            {
                colorType = colorType,
                blockFlag = BlockFlag.Normal,
                position = position,
            };
        }
    }
}

