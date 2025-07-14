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
            _unmanagedBockDataArray = new BlockData[rows * columns];
            _unmanagedTileDataArray = new TileData[rows * columns];
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

