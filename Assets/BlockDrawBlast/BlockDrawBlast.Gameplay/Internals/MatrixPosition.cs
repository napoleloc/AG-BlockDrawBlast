using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    public readonly record struct MatrixPosition(int RowIndex, int ColumnIndex)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid(int rows, int columns)
        {
            return RowIndex >= 0 && RowIndex < rows && ColumnIndex >= 0 && ColumnIndex < columns;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToIndex(int columns)
        {
            return RowIndex * columns + ColumnIndex;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int2 ToInt2()
        {
            return new int2(ColumnIndex, RowIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MatrixPosition FromIndex(int index, int column)
        {
            return new MatrixPosition(index / column, index % column);
        }
        
        
    }
}