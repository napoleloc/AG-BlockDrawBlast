using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TileData
    {
        public int row;
        public int column;
        public TileState tileState;
        public float3 worldPosition;
    }
}

