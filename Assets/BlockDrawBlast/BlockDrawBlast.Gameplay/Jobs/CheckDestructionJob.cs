using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile]
    public struct CheckDestructionJob : IJob
    {
        [ReadOnly] public NativeArray<BlockData> blockDataArray;
        [ReadOnly] public NativeArray<TileData> tileDataArray;
        [ReadOnly] public int rows;
        [ReadOnly] public int columns;

        public NativeList<int> destructionIndices;
        public NativeArray<bool> destructionResult;
        
        public void Execute()
        {
            destructionIndices.Clear();
            destructionResult[0] = false;

            for (int row = 0; row < rows; row++)
            {
                if(CheckRowDestruction(row) == false) continue;
                
                AddRowToDestruction(row);
                destructionResult[0] = true;
            }

            for (int column = 0; column < columns; column++)
            {
                if(CheckColumnDestruction(column) == false) continue;
                
                AddColumnToDestruction(column);
                destructionResult[0] = true;
            }
        }

        private bool CheckRowDestruction(int row)
        {
            var firstBlockIndex = row * columns;
            var firstTileData = tileDataArray[firstBlockIndex];
            
            if((firstTileData.flag & TileFlag.Occupied) == 0) return false;
            
            var firstBlockData = blockDataArray[firstBlockIndex];
            var firstColorType = firstBlockData.colorType;

            for (int column = 0; column < columns; column++)
            {
                var index = row * columns + column;
                var tile = tileDataArray[index];

                if ((tile.flag & TileFlag.Occupied) == 0) return false;
                
                var block = blockDataArray[index];
                if(block.colorType != firstColorType) return false;
            }
            
            return true;
        }
        
        private bool CheckColumnDestruction(int column)
        {
            var firstBlockIndex = column;
            var firstTileData = tileDataArray[firstBlockIndex];
            
            if((firstTileData.flag & TileFlag.Occupied) == 0) return false;
            
            var firstBlockData = blockDataArray[firstBlockIndex];
            var firstColorType = firstBlockData.colorType;

            for (int row = 0; row < rows; row++)
            {
                var index = row * columns + column;
                var tile = tileDataArray[index];
                
                if ((tile.flag & TileFlag.Occupied) == 0) return false;
                
                var block = blockDataArray[index];
                if(block.colorType != firstColorType) return false;
            }
            
            return true;
        }
        
        private void AddRowToDestruction(int row)
        {
            for (int column = 0; column < columns; column++)
            {
                var index = row * columns + column;
                if (destructionIndices.Contains(index) == false)
                {
                    destructionIndices.Add(index);
                }
            }
        }

        private void AddColumnToDestruction(int column)
        {
            for (int row = 0; row < rows; row++)
            {
                var index = row * columns + column;
                if (destructionIndices.Contains(index) == false)
                {
                    destructionIndices.Add(index);
                }
            }
        }
    }
}

