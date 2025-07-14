using System.Runtime.CompilerServices;

namespace BlockDrawBlast.Gameplay
{
    public static class BlockFlags
    {
        public const byte ACTIVE = 1 << 0;        // Block is active
        public const byte DIRTY = 1 << 1;         // Block needs update
        public const byte LOCKED = 1 << 2;        // Block is locked
        public const byte DAMAGED = 1 << 3;       // Block is damaged
        public const byte COLLECTED = 1 << 4;     // Key was collected
        public const byte MARKED_FOR_REMOVAL = 1 << 5;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag(byte flags, byte flag)
        {
            return (flags & flag) != 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SetFlag(byte flags, byte flag)
        {
            return (byte)(flags | flag);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ClearFlag(byte flags, byte flag)
        {
            return (byte)(flags & ~flag);
        }
    }

}
