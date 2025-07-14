using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile(OptimizeFor = OptimizeFor.Performance)]
    public struct BlockProcessingJob : IJobParallelFor
    {
        // Input data (read-only)
        [ReadOnly] public NativeArray<int2> positions;
        [ReadOnly] public NativeArray<ColorType> colors;
        [ReadOnly] public NativeArray<BlockType> types;
        [ReadOnly] public NativeArray<int> healths;
        [ReadOnly] public NativeArray<int> keyRequirements;
        [ReadOnly] public NativeArray<byte> flags;
        
        // Shared data
        [ReadOnly] public NativeArray<int> keyInventory;
        [ReadOnly] public float deltaTime;
        [ReadOnly] public int gridWidth;
        [ReadOnly] public int gridHeight;

        // Output data
        [WriteOnly] public NativeArray<int> newHealths;
        [WriteOnly] public NativeArray<byte> newFlags;
        [WriteOnly] public NativeArray<float> interactionScores;
        
        public void Execute(int index)
        {
            // Early exit for inactive blocks
            if (BlockFlags.HasFlag(flags[index], BlockFlags.ACTIVE) == false)
            {
                newHealths[index] = healths[index];
                newFlags[index] = flags[index];
                interactionScores[index] = 0f;
                return;
            }

            var blockType = types[index];
            var position = positions[index];
            var health = healths[index];
            var currentFlags = flags[index];

            // Process based on block type
            var result = ProcessBlockByType(blockType, position, health, currentFlags, index);
            
            // Write results
            newHealths[index] = result.health;
            newFlags[index] = result.flags;
            interactionScores[index] = result.score;

        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BlockProcessResult ProcessBlockByType(BlockType type, int2 position, int health, byte currentFlags, int index)
        {
            return type switch
            {
                BlockType.Normal => ProcessNormalBlock(position, health, currentFlags),
                BlockType.Key => ProcessKeyBlock(position, health, currentFlags, index),
                BlockType.Locked => ProcessLockedBlock(position, health, currentFlags, index),
                BlockType.MultiHit => ProcessMultiHitBlock(position, health, currentFlags),
                _ => new BlockProcessResult { health = health, flags = currentFlags, score = 0f }
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BlockProcessResult ProcessNormalBlock(int2 position, int health, byte currentFlags)
        {
            // Normal blocks don't need special processing
            return new BlockProcessResult 
            { 
                health = health, 
                flags = currentFlags, 
                score = 1.0f 
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BlockProcessResult ProcessKeyBlock(int2 position, int health, byte currentFlags, int index)
        {
            // Key blocks can be collected
            var score = 10.0f;
            var newFlags = BlockFlags.SetFlag(currentFlags, BlockFlags.COLLECTED);
            
            return new BlockProcessResult 
            { 
                health = health, 
                flags = newFlags, 
                score = score 
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BlockProcessResult ProcessLockedBlock(int2 position, int health, byte currentFlags, int index)
        {
            var color = colors[index];
            var keyReq = keyRequirements[index];
            var colorIndex = (int)color;
            
            // Check if we can unlock
            if (colorIndex >= 0 && colorIndex < keyInventory.Length)
            {
                var availableKeys = keyInventory[colorIndex];
                if (availableKeys >= keyReq)
                {
                    // Can unlock
                    var newFlags = BlockFlags.ClearFlag(currentFlags, BlockFlags.LOCKED);
                    return new BlockProcessResult 
                    { 
                        health = health, 
                        flags = newFlags, 
                        score = 15.0f 
                    };
                }
            }
            
            return new BlockProcessResult 
            { 
                health = health, 
                flags = currentFlags, 
                score = 0f 
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BlockProcessResult ProcessMultiHitBlock(int2 position, int health, byte currentFlags)
        {
            // Multi-hit blocks lose health over time
            var newHealth = health;
            var newFlags = currentFlags;
            
            if (BlockFlags.HasFlag(currentFlags, BlockFlags.DAMAGED))
            {
                newHealth = math.max(0, health - 1);
                if (newHealth <= 0)
                {
                    newFlags = BlockFlags.SetFlag(newFlags, BlockFlags.MARKED_FOR_REMOVAL);
                }
            }
            
            return new BlockProcessResult 
            { 
                health = newHealth, 
                flags = newFlags, 
                score = newHealth > 0 ? 5.0f : 20.0f 
            };
        }
        
        private struct BlockProcessResult
        {
            public int health;
            public byte flags;
            public float score;
        }
    }
}

