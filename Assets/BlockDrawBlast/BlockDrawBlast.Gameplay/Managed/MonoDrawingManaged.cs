using System;
using EncosyTower.Collections;
using EncosyTower.Ids;
using EncosyTower.Types;
using EncosyTower.Vaults;
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

        private MatrixPosition _lastCoordDrawBlock;
        private MatrixPosition _firstCoordUndoDrawBlock;
        
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
            
            _lastCoordDrawBlock.Dispose();
            _firstCoordUndoDrawBlock.Dispose();
            
            if(_monoMatrixManaged.IsValidPosition(position) == false) return;
            if(TryProcessDrawingAtPosition(position) == false) return;
            
            _isDrawing = true;
        }

        public void ContinueDrawing(MatrixPosition position)
        {
            if(_isDrawing == false) return;

            var validPosition = _monoMatrixManaged.IsValidPosition(position)
                ? position
                : _monoMatrixManaged.ClampPosition(position);

            TryProcessDrawingAtPosition(validPosition);
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

        private bool TryProcessDrawingAtPosition(MatrixPosition position)
        {
            var drawingBlockContext = new DrawingBlockContext()
            {
                position = position,
                colorType = ColorType.Pink
            };
            
            return _monoMatrixManaged.TryPlaceBlock(position, drawingBlockContext);
        }
        
        private void CancelDrawingBlock()
        {
            if (_currentDrawingBlocks.Count < 1)
            {
                return;
            }
        }
    }
}

