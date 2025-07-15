using System;

namespace BlockDrawBlast.Gameplay
{
    [Flags]
    public enum TileFlag : byte
    {
        None = 0,
        Occupied = 1 << 0,
        Locked = 1 << 1,
        HasKey = 1 << 2,
    }
}

