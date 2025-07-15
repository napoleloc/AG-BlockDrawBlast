using System;
using System.Runtime.CompilerServices;
using Unity.Burst;

namespace BlockDrawBlast.Gameplay
{
    [BurstCompile]
    public static class Validator
    {
        public static bool IsValidDrawPosition(
            MatrixPosition position
            , DrawingPattern pattern
            , ReadOnlySpan<BlockData> unmanagedDataArray
            , int rows
            , int columns)
        {
            if (IsPositionInBounds(position, rows, columns) == false)
            {
                return false;
            }
            
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositionInBounds(MatrixPosition position, int rows, int columns)
        {
            return position.IsValid(rows, columns);
        }
    }
    
    
}

