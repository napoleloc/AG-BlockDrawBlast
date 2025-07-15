using System;
using System.Runtime.CompilerServices;

namespace BlockDrawBlast.Gameplay
{
    internal static class Constants
    {
        private const string PREFAB = "prefab";

        public const string BLOCK_NORMAL = $"{PREFAB}-block-normal";
        public const string BLOCK_KEY = $"{PREFAB}-block-key";
        public const string BLOCK_LOCKED = $"{PREFAB}-block-locked";
        public const string BLOCK_EXPLOSIVE = $"{PREFAB}-block-explosive";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetBlockPrefabPath(BlockType blockType)
        {
            return blockType switch
            {
                BlockType.Normal => BLOCK_NORMAL,
                BlockType.Key => BLOCK_KEY,
                BlockType.Locked => BLOCK_LOCKED,
                BlockType.Explosive => BLOCK_EXPLOSIVE,
                _ => string.Empty
            };
        }
      
    }
}
