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
        
        public float3 WorldPosition { get; init; }
        
        // Neighbor checks
        public bool IsAdjacentTo(BlockData other)
        {
            var rowDiff = math.abs(Row - other.Row);
            var colDiff = math.abs(Column - other.Column);
            return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
        }
    }
}
