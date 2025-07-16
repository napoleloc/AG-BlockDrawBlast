using System;

namespace BlockDrawBlast.Gameplay
{
    [Flags]
    public enum BlockFlag : byte
    {
        None = 0,
        Normal = 1 << 0,
        Key = 1 << 1,
        Locked = 1 << 2,
        Explosive = 1 << 3,
    }
}

