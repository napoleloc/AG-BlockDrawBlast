using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile]
    public struct DrawingPatternJob : IJob
    {
        [ReadOnly] public NativeArray<MatrixPosition> inputDrawingPath;
        [ReadOnly] public int2 inputCoord;
        [ReadOnly] public int2 matrixSize;
        
        [WriteOnly] public NativeArray<bool> valid;
        
        public void Execute()
        {
            if(IsValidPosition(inputCoord) == false) return;
            
            if(IsContinuousPattern() == false) return;

            valid[0] = true;
        }
        
        private bool IsContinuousPattern()
        {
            var lenght = inputDrawingPath.Length;

            for (int index = 0; index < lenght; index++)
            {
                if (IsAdjacent(inputCoord, inputDrawingPath[index].ToInt2()))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private static bool IsAdjacent(int2 left, int2 right)
        {
            var rowDiff = math.abs(left.y - right.y);
            var colDiff = math.abs(left.x - right.x);
    
            // Adjacent = 8 directions  
            return (rowDiff <= 1 && colDiff <= 1) && (rowDiff + colDiff > 0);
        }
        
        private bool IsValidPosition(int2 position)
        {
            return position.x >= 0 && position.x < matrixSize.x && 
                   position.y >= 0 && position.y < matrixSize.y;
        }
    }
}

