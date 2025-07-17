using System;
using System.Runtime.CompilerServices;
using EncosyTower.Collections;
using EncosyTower.Ids;
using EncosyTower.Types;
using EncosyTower.Vaults;
using Unity.Mathematics;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoDrawingManaged : MonoBehaviour
    {
        public static readonly Id<MonoDrawingManaged> TypeId = Type<MonoDrawingManaged>.Id;
        
        [SerializeField] private MonoMatrixManaged _monoMatrixManaged;
        [SerializeField] private MonoStackManaged _monoStackManaged;
        [SerializeField] private DrawingPattern _drawingPattern;
        
        private readonly FasterList<DrawingBlockContext> _currentDrawingBlocks = new();
        private bool _isDrawing;

        public System.Action OnDrawingCompleted;
        public System.Action OnDrawingCanceled;
        public System.Action OnDrawingFailed;
        public System.Action<ReadOnlyMemory<DrawingBlockContext>> OnBlocksPlaced;
        public System.Action<ReadOnlyMemory<DrawingBlockContext>> OnDrawingUpdated;

        private void Awake()
        {
            GlobalObjectVault.TryAdd(TypeId, this);
        }
        
        private void OnDestroy()
        {
            GlobalObjectVault.TryRemove(TypeId, out _);
        }

        public void StartDrawing(MatrixPosition position)
        {
            if(_isDrawing) return;
            
            if(_monoMatrixManaged.IsValidPosition(position) == false) return;

            _isDrawing = true;
        }

        public void ContinueDrawing(MatrixPosition position)
        {
            if(_isDrawing == false) return;

            var validPosition = _monoMatrixManaged.IsValidPosition(position)
                ? position
                : _monoMatrixManaged.ClampPosition(position);

            var drawingBlockContext = new DrawingBlockContext()
            {
                position = position,
                colorType = ColorType.Pink
            };
            
            if(_monoMatrixManaged.TryPlaceBlock(position, drawingBlockContext) == false) return;            
            
        }

        public void EndDrawing()
        {
            if(_isDrawing == false) return;
            
            _isDrawing = false;

            if (_currentDrawingBlocks.Count == 0)
            {
                OnDrawingFailed?.Invoke();
                return;
            }
            
            _currentDrawingBlocks.Clear();
        }
        

        private bool IsAdjacentToLastBlock(MatrixPosition position)
        {
            if(_currentDrawingBlocks.Count == 0) return false;
            
            var lastPosition = _currentDrawingBlocks[^1].position;
            return IsAdjacent(position, lastPosition);
        }

        private bool IsAdjacentToAnyDrawnBlock(MatrixPosition position)
        {
            if(_currentDrawingBlocks.Count == 1) return true;
            
            var currentDrawingBlocks = _currentDrawingBlocks.AsSpan();
            var length = currentDrawingBlocks.Length;

            for (int index = 0; index < length; index++)
            {
                if (IsAdjacent(position, currentDrawingBlocks[index].position))
                {
                    return true;
                }
                
            }
            
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAdjacent(MatrixPosition pos1, MatrixPosition pos2)
        {
            var rowDiff = math.abs(pos1.RowIndex - pos2.RowIndex);
            var colDiff = math.abs(pos1.ColumnIndex - pos2.ColumnIndex);
    
            // Adjacent = 8 directions  
            return (rowDiff <= 1 && colDiff <= 1) && (rowDiff + colDiff > 0);
        }
    }
}

