using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile]
    public struct ProcessDestructionJob : IJob
    {
        public NativeArray<BlockData> blockDataArray;
        public NativeArray<TileData> tileDataArray;

        [ReadOnly] public NativeArray<int> destructionIndices;
        [ReadOnly] public int rows;
        [ReadOnly] public int columns;

        public NativeArray<int> keyCollected;
        public NativeList<int> adjacentAffectedIndices;
        
        public void Execute()
        {
            keyCollected[0] = 0;
            adjacentAffectedIndices.Clear();
        }

        private void ProcessBlockDestruction(int index)
        {
            var blockData = blockDataArray[index];
            var tileData = tileDataArray[index];

            if (blockData.blockType == BlockType.Key)
            {
                keyCollected[0]++;
            }

            blockDataArray[index] = default;

            tileData.flag &= ~(TileFlag.Occupied | TileFlag.Locked | TileFlag.HasKey);
            tileDataArray[index] = tileData;
        }

        private void ProcessAdjacentBlocks(int centerIndex)
        {
            var centerPosition = MatrixPosition.FromIndex(centerIndex, columns);

            for (int deltaRow = -1; deltaRow <= 1; deltaRow++)
            {
                for (int deltaCol = -1; deltaCol <= 1; deltaCol++)
                {
                    if(deltaCol == 0 && deltaRow == 0) continue;
                    
                    var adjacentPosition = new MatrixPosition()
                    {
                        RowIndex = centerPosition.RowIndex +deltaRow,
                        ColumnIndex = centerPosition.ColumnIndex + deltaCol,
                    };
                    
                    if(adjacentPosition.IsValid(rows, columns) == false) continue;
                    
                    var adjacentIndex = adjacentPosition.ToIndex(columns);
                    var adjacentTileData = tileDataArray[adjacentIndex];
                    
                    if((adjacentTileData.flag & TileFlag.Occupied) == 0) continue;
                    
                    var adjacentBlockData = blockDataArray[adjacentIndex];

                    switch (adjacentBlockData.blockType)
                    {
                        case BlockType.Key:
                        {
                            keyCollected[0]++;
                            DestroyBlock(adjacentIndex);
                            break;
                        }

                        case BlockType.Explosive:
                        {
                            DestroyBlock(adjacentIndex);
                            adjacentAffectedIndices.Add(adjacentIndex);
                            break;
                        }

                        case BlockType.Locked:
                        {
                            if (adjacentBlockData.requiredKeys == 0)
                            {
                                DestroyBlock(adjacentIndex);
                            }
                            break;
                        }
                    }
                    
                }
            }
            
            
        }

        private void DestroyBlock(int index)
        {
            blockDataArray[index] = default;
            
            var tileData = tileDataArray[index];
            tileData.flag &= ~(TileFlag.Occupied | TileFlag.Locked | TileFlag.HasKey);
            tileDataArray[index] = tileData;
        }
    }
    
}

