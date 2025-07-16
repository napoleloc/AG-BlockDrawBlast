using System;
using BlockDrawBlast.Database;
using BlockDrawBlast.Gameplay;
using Unity.Collections;

namespace BlockDrawBlast.Extensions
{
    public static class ConvertDataHelper
    {
        public static NativeArray<TileData> ConvertTileDataBurst(ReadOnlySpan<LevelTileData> source)
        {
            if (source.Length == 0)
                return new NativeArray<TileData>(0, Allocator.Temp);

            var result = new NativeArray<TileData>(source.Length, Allocator.Temp);
            
            for (int i = 0; i < source.Length; i++)
            {
                var tile = source[i];
                result[i] = new TileData
                {
                    position = new MatrixPosition(tile.row, tile.column),
                    flag = tile.flag
                };
            }
            
            return result;
        }

        public static NativeArray<BlockData> ConvertBlockDataBurst(ReadOnlySpan<LevelBlockData> source)
        {
            if (source.Length == 0)
                return new NativeArray<BlockData>(0, Allocator.Temp);

            var result = new NativeArray<BlockData>(source.Length, Allocator.Temp);
            
            for (int i = 0; i < source.Length; i++)
            {
                var block = source[i];
                result[i] = new BlockData
                {
                    position = new MatrixPosition(block.row, block.column),
                    colorType = block.colorType,
                    blockFlag = block.blockFlag,
                    requiredKeys = block.requiredKeys
                };
            }
            
            return result;
        }

        public static NativeArray<StackBlockData> ConvertStackDataBurst(ReadOnlySpan<LevelStackBlockData> source)
        {
            if (source.Length == 0)
                return new NativeArray<StackBlockData>(0, Allocator.Temp);

            var result = new NativeArray<StackBlockData>(source.Length, Allocator.Temp);
            
            for (int i = 0; i < source.Length; i++)
            {
                var stackBlock = source[i];
                result[i] = new StackBlockData
                {
                    colorType = stackBlock.colorType,
                    count = stackBlock.count
                };
            }
            
            return result;
        }
    }
}

