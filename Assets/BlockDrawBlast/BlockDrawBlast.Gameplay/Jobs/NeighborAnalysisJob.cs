using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile(OptimizeFor = OptimizeFor.Performance)]
    public struct NeighborAnalysisJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<int2> positions;
        [ReadOnly] public NativeArray<ColorType> colors;
        [ReadOnly] public NativeArray<BlockType> types;
        [ReadOnly] public NativeArray<byte> flags;
        [ReadOnly] public NativeArray<int> gridToIndex;
        [ReadOnly] public int gridWidth;
        [ReadOnly] public int gridHeight;
    
        [WriteOnly] public NativeArray<int> neighborCounts;
        [WriteOnly] public NativeArray<float> clusterScores;
        
        // Temp arrays for neighbor processing
        [NativeDisableParallelForRestriction]
        public NativeArray<int> neighborArray;
        
        public void Execute(int index)
        {
            if (BlockFlags.HasFlag(flags[index], BlockFlags.ACTIVE) == false)
            {
                neighborCounts[index] = 0;
                clusterScores[index] = 0f;
                return;
            }
            
            var position = positions[index];
            var color = colors[index];
            var blockType = types[index];
            
            // Get neighbors using optimized lookup
            var neighbors = GetNeighbors(position, index);
            var sameColorCount = CountSameColorNeighbors(neighbors, color);
        
            neighborCounts[index] = neighbors.Length;
            clusterScores[index] = CalculateClusterScore(sameColorCount, blockType);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private NativeArray<int> GetNeighbors(int2 position, int startIndex)
        {
            var start = startIndex * 8;
            var neighborCount = 0;
            
            var offsetArray = new NativeArray<int2>(4, Allocator.Temp);
            offsetArray[0] = new int2(-1, 0);
            offsetArray[1] = new int2(1, 0);
            offsetArray[2] = new int2(0, -1);
            offsetArray[3] = new int2(0, 1);
            
            for (int i = 0; i < 4; i++)
            {
                var neighborPos = position + offsetArray[i];
                if (IsValidPosition(neighborPos))
                {
                    var gridIndex = GetGridIndex(neighborPos);
                    var blockIndex = gridToIndex[gridIndex];
                
                    if (blockIndex >= 0)
                    {
                        neighborArray[start + neighborCount] = blockIndex;
                        neighborCount++;
                    }
                }
            }
        
            offsetArray.Dispose();
        
            return neighborArray.GetSubArray(start, neighborCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CountSameColorNeighbors(NativeArray<int> neighbors, ColorType targetColor)
        {
            var count = 0;
            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborIndex = neighbors[i];
                if (colors[neighborIndex] == targetColor)
                {
                    count++;
                }
            }
            return count;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalculateClusterScore(int sameColorCount, BlockType blockType)
        {
            var baseScore = sameColorCount * 2.0f;
        
            // Type-specific bonuses
            var typeMultiplier = blockType switch
            {
                BlockType.Key => 1.5f,
                BlockType.Locked => 0.8f,
                BlockType.MultiHit => 1.2f,
                _ => 1.0f
            };
        
            return baseScore * typeMultiplier;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidPosition(int2 pos)
        {
            return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetGridIndex(int2 pos)
        {
            return pos.y * gridWidth + pos.x;
        }
    }
}

