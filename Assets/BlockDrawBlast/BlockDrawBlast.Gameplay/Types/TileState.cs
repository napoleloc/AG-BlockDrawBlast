namespace BlockDrawBlast.Gameplay
{
    public enum TileState : byte
    {
        Empty,      // Tile trống, có thể đặt block
        Occupied,   // Tile đã có block
        Blocked     // Tile không thể đặt block (obstacle, wall...)
    }
}

