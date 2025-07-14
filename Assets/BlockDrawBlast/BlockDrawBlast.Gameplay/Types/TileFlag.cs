namespace BlockDrawBlast.Gameplay
{
    public enum TileFlag : byte
    {
        None = 0,
        Occupied = 1 << 0,
        Locked = 1 << 1,
        HasKey = 1 << 2,
        Destructible = 1 << 3,
        ValidPlacement = 1 << 4
    }
}

