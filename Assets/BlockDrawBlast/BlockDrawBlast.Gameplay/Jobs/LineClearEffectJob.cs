using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile]
    public struct LineClearEffectJob : IJob
    {
        [ReadOnly] public NativeArray<BlockData> inputBlockDataArray;
        [ReadOnly] public NativeArray<TileData> inputTileDataArray;
        [ReadOnly] public NativeArray<int> inputClearBlockIndices;
        [ReadOnly] public int inputRows;
        [ReadOnly] public int inputColumns;
        
        [WriteOnly] public NativeList<int> triggeredExplosiveIndices;
        [WriteOnly] public NativeArray<int> triggeredKeyCount;
        
        public void Execute()
        {
            for (int i = 0; i < inputClearBlockIndices.Length; i++)
            {
                int blockIndex = inputClearBlockIndices[i];
                ProcessClearedBlock(blockIndex);
                CheckAdjacentBlocks(blockIndex);
            }

        }

        private void ProcessClearedBlock(int index)
        {
            var inputBlockData = inputBlockDataArray[index];

            if (Hint.Unlikely(inputBlockData.blockFlag == BlockFlag.Key))
            {
                triggeredKeyCount[0]++;
            }
        }

        private void CheckAdjacentBlocks(int centerIndex)
        {
            var centerPosition = IndexToPosition(centerIndex);
            
            // Check Up
            CheckAdjacentPosition(centerPosition + new int2(0, -1));
            // Check Down  
            CheckAdjacentPosition(centerPosition + new int2(0, 1));
            // Check Left
            CheckAdjacentPosition(centerPosition + new int2(-1, 0));
            // Check Right
            CheckAdjacentPosition(centerPosition + new int2(1, 0));
        }

        private void CheckAdjacentPosition(int2 position)
        {
            if(Hint.Unlikely(IsValidPosition(position) == false)) return;
            
            var adjacentIndex = PositionToIndex(position);
            ProcessAdjacentBlock(adjacentIndex);
        }

        private void ProcessAdjacentBlock(int index)
        {
            var inputTileData = inputTileDataArray[index];
            if(Hint.Likely((inputTileData.flag & TileFlag.Occupied) == 0)) return;
            
            var inputBlockData = inputBlockDataArray[index];
            if(Hint.Likely(inputBlockData.blockFlag == BlockFlag.Explosive))
            {
                triggeredExplosiveIndices.Add(index);
            }
        }
        
        private bool IsValidPosition(int2 position)
        {
            return position.x >= 0 && position.x < inputColumns && 
                   position.y >= 0 && position.y < inputRows;
        }
        
        // Convention: int2(column, row) = int2(x, y)
        private int2 IndexToPosition(int index)
        {
            return new int2(index % inputColumns, index / inputColumns);
        }
        
        private int PositionToIndex(int2 position)
        {
            return position.y * inputColumns + position.x;
        }

    }
}

