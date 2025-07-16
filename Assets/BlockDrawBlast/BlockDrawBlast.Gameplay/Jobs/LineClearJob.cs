using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile]
    public struct LineClearJob : IJob
    {
        [ReadOnly] public NativeArray<BlockData> inputBlockDataArray;
        [ReadOnly] public NativeArray<TileData> inputTileDataArray;
        [ReadOnly] public int inputRows;
        [ReadOnly] public int inputColumns;
        
        [WriteOnly] public NativeList<int> outputClearBlockIndices;
        
        public void Execute()
        {
            for (int row = 0; row < inputRows; row++)
            {
                if(IsRowComplete(row) == false) continue;
                
                AddRowBlocksToOutput(row);
            }

            for (int column = 0; column < inputColumns; column++)
            {
                if(IsColumnComplete(column) == false) continue;
                
                AddColumnBlocksToOutput(column);
            }
        }

        public bool IsRowComplete(int row)
        {
            var firstBlockIndex = row * inputColumns;
            var firstTileData = inputTileDataArray[firstBlockIndex];
            
            if((firstTileData.flag & TileFlag.Occupied) == 0) return false;
            
            var firstBlockData = inputBlockDataArray[firstBlockIndex];
            var firstColorType = firstBlockData.colorType;

            for (int column = 0; column < inputColumns; column++)
            {
                var index = GetBlockIndex(row, column);
                
                var inputTileData = inputTileDataArray[index];
                if ((inputTileData.flag & TileFlag.Occupied) == 0) return false;
                
                var inputBlockData = inputBlockDataArray[index];
                if(inputBlockData.colorType != firstColorType) return false;
            }
            
            return true;
        }
        
        public bool IsColumnComplete(int column)
        {
            var firstIndex = column;
            var firstTileData = inputTileDataArray[firstIndex];
            
            if((firstTileData.flag & TileFlag.Occupied) == 0) return false;
            
            var firstBlockData = inputBlockDataArray[firstIndex];
            var firstColorType = firstBlockData.colorType;

            for (int row = 0; row < inputRows; row++)
            {
                var index = GetBlockIndex(row, column);
                
                var inputTileData = inputTileDataArray[index];
                if ((inputTileData.flag & TileFlag.Occupied) == 0) return false;
                
                var inputBlockData = inputBlockDataArray[index];
                if(inputBlockData.colorType != firstColorType) return false;
            }
            
            return true;
        }
        

        private void AddRowBlocksToOutput(int row)
        {
            for (int column = 0; column < inputColumns; column++)
            {
                var index = GetBlockIndex(row, column);
                
                outputClearBlockIndices.Add(index);
            }
        }
        
        private void AddColumnBlocksToOutput(int column)
        {
            for (int row = 0; row < inputRows; row++)
            {
                var index = GetBlockIndex(row, column);
                
                outputClearBlockIndices.Add(index);
            }
        }
        
        private int GetBlockIndex(int row, int column)
        {
            return row * inputColumns + column;
        }
    }
}

