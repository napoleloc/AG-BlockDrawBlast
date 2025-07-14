using System;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoMatrixManager : MonoBehaviour, IDisposable
    {
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        
        private BlockData[] _unmanagedBockDataArray;
        private TileData[] _unmanagedTileDataArray;
        
        private bool _isInitialized;

        public void Initialize(int rows, int columns)
        {
            var initialCapacity = rows * columns;
            
            _unmanagedBockDataArray = new BlockData[initialCapacity];
            _unmanagedTileDataArray = new TileData[initialCapacity];
        }

        private bool TryPlaceBlock(MatrixPosition position, out BlockData blockData)
        {
            ref var blockDataRef = ref _unmanagedBockDataArray[position.ToIndex(_columns)];
            blockData = blockDataRef;
            return true;
        }
        
        public void Dispose()
        {
        }
    }
}

