using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile]
    public struct LineBlastJob : IJob 
    {
        [ReadOnly] public NativeArray<BlockData> blocks;
        [ReadOnly] public int gridRows;
        [ReadOnly] public int gridColumns;
        
        [WriteOnly] public NativeList<int> completedRows;
        [WriteOnly] public NativeList<int> completedColumns;

        public void Execute()
        {
            var gridColorMatrix = new NativeArray<ColorType>(gridRows * gridColumns, Allocator.Temp);
            var gridOccupancyMatrix = new NativeArray<bool>(gridRows * gridColumns, Allocator.Temp);

            InitializeGridMatrices(gridColorMatrix, gridOccupancyMatrix);
            
            PopulateGridFromBlocks(gridColorMatrix, gridOccupancyMatrix);
            
            // Check for completed lines
            CheckForCompletedRows(gridColorMatrix, gridOccupancyMatrix);
            CheckForCompletedColumns(gridColorMatrix, gridOccupancyMatrix);
            
            // Cleanup
            gridColorMatrix.Dispose();
            gridOccupancyMatrix.Dispose();
        }

        private void InitializeGridMatrices(NativeArray<ColorType> colorMatrix, NativeArray<bool> occupancyMatrix)
        {
            for (int i = 0; i < colorMatrix.Length; i++)
            {
                colorMatrix[i] = ColorType.Red; // Default color
                occupancyMatrix[i] = false;
            }
        }

        private void PopulateGridFromBlocks(NativeArray<ColorType> colorMatrix, NativeArray<bool> occupancyMatrix)
        {
            for (var blockIndex = 0; blockIndex < blocks.Length; blockIndex++)
            {
                var block = blocks[blockIndex];
                if (Hint.Likely(IsValidGridPosition(block.Row, block.Column)))
                {
                    var matrixIndex = GetMatrixIndex(block.Row, block.Column);
                    colorMatrix[matrixIndex] = block.ColorType;
                    occupancyMatrix[matrixIndex] = true;
                }
            }
        }

        private void CheckForCompletedRows(ReadOnlySpan<ColorType> colorMatrix, ReadOnlySpan<bool> occupancyMatrix)
        {
            for (int rowIndex = 0; rowIndex < gridRows; rowIndex++)
            {
                if (Hint.Unlikely(IsRowCompletedWithSameColor(colorMatrix, occupancyMatrix, rowIndex)))
                {
                    completedRows.Add(rowIndex);
                }
            }
        }

        private void CheckForCompletedColumns(ReadOnlySpan<ColorType> colorMatrix, ReadOnlySpan<bool> occupancyMatrix)
        {
            for (int columnIndex = 0; columnIndex < gridColumns; columnIndex++)
            {
                if (Hint.Unlikely(IsColumnCompletedWithSameColor(colorMatrix, occupancyMatrix, columnIndex)))
                {
                    completedColumns.Add(columnIndex);
                }
            }
        }

        private bool IsRowCompletedWithSameColor(
            ReadOnlySpan<ColorType> colorMatrix,
            ReadOnlySpan<bool> occupancyMatrix,
            int rowIndex)
        {
            if (IsRowFullyOccupied(occupancyMatrix, rowIndex) == false)
            {
                return false;
            }

            // Check if all blocks in row have same color
            return AreAllBlocksInRowSameColor(colorMatrix, rowIndex);
        }
        
        private bool IsColumnCompletedWithSameColor(
            ReadOnlySpan<ColorType> colorMatrix,
            ReadOnlySpan<bool> occupancyMatrix,
            int columnIndex)
        {
            if (IsColumnFullyOccupied(occupancyMatrix, columnIndex) == false)
            {
                return false;
            }

            // Check if all blocks in column have same color
            return AreAllBlocksInColumnSameColor(colorMatrix, columnIndex);
        }

        private bool IsRowFullyOccupied(ReadOnlySpan<bool> occupancyMatrix, int rowIndex)
        {
            for (int columnIndex = 0; columnIndex < gridColumns; columnIndex++)
            {
                var matrixIndex = GetMatrixIndex(rowIndex, columnIndex);
                if (Hint.Unlikely(occupancyMatrix[matrixIndex] == false))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsColumnFullyOccupied(ReadOnlySpan<bool> occupancyMatrix, int columnIndex)
        {
            for (int rowIndex = 0; rowIndex < gridRows; rowIndex++)
            {
                var matrixIndex = GetMatrixIndex(rowIndex, columnIndex);
                if (Hint.Unlikely(occupancyMatrix[matrixIndex] == false))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AreAllBlocksInRowSameColor(ReadOnlySpan<ColorType> colorMatrix, int rowIndex)
        {
            var firstBlockIndex = GetMatrixIndex(rowIndex, 0);
            var expectedColor = colorMatrix[firstBlockIndex];
            
            for (int columnIndex = 1; columnIndex < gridColumns; columnIndex++)
            {
                var matrixIndex = GetMatrixIndex(rowIndex, columnIndex);
                if (Hint.Unlikely(colorMatrix[matrixIndex] != expectedColor))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AreAllBlocksInColumnSameColor(ReadOnlySpan<ColorType> colorMatrix, int columnIndex)
        {
            var firstBlockIndex = GetMatrixIndex(0, columnIndex);
            var expectedColor = colorMatrix[firstBlockIndex];
            
            for (int rowIndex = 1; rowIndex < gridRows; rowIndex++)
            {
                var matrixIndex = GetMatrixIndex(rowIndex, columnIndex);
                if (Hint.Unlikely(colorMatrix[matrixIndex] != expectedColor))
                {
                    return false;
                }
            }
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetMatrixIndex(int row, int column)
        {
            return row * gridColumns + column;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidGridPosition(int row, int column)
        {
            return row >= 0 && row < gridRows && column >= 0 && column < gridColumns;
        }
    }
}