using System.Runtime.InteropServices;

namespace BlockDrawBlast.Gameplay
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TileData
    {
        public MatrixPosition position;
        public TileFlag flag;
    }
}

