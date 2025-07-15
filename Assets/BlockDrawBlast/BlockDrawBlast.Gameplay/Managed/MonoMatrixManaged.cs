using System;
using EncosyTower.Logging;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoMatrixManaged : MonoBehaviour, IDisposable
    {
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        
        private NativeArray<BlockData> _unmanagedBockDataArray;
        private NativeArray<TileData> _unmanagedTileDataArray;
        private NativeArray<int> _keyCountArray;
        
        private bool _isInitialized;

        public void Initialize(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            
            var initialCapacity = rows * columns;
            
            if(_unmanagedBockDataArray.IsCreated) _unmanagedBockDataArray.Dispose();
            if (_unmanagedTileDataArray.IsCreated) _unmanagedTileDataArray.Dispose();
            if(_keyCountArray.IsCreated) _keyCountArray.Dispose();
            
            _unmanagedBockDataArray = new NativeArray<BlockData>(initialCapacity, Allocator.Persistent);
            _unmanagedTileDataArray = new NativeArray<TileData>(initialCapacity, Allocator.Persistent);
            _keyCountArray = new NativeArray<int>(1, Allocator.Persistent);
            
            // Initialize tiles
            for (var i = 0; i < initialCapacity; i++)
            {
                var position = MatrixPosition.FromIndex(i, _columns);
                _unmanagedTileDataArray[i] = new TileData
                {
                    position = position,
                    flag = TileFlag.None
                };
            }
            
            _isInitialized = true;
        }

        public bool TryPlaceBlock(MatrixPosition position, BlockData blockData)
        {
            if (_isInitialized == false)
            {
                DevLoggerAPI.LogError("Matrix is not initialized.");
                return false;
            }
            
            var index = position.ToIndex(_columns);
            if (index < 0 || index >= _unmanagedBockDataArray.Length)
            {
                DevLoggerAPI.LogError($"Invalid index: {index}");
                return false;
            }
            
            var unmanagedTileData = _unmanagedTileDataArray[index];
            if ((unmanagedTileData.flag & TileFlag.Occupied) != 0)
            {
                return false;
            }
            
            blockData.position = position;
            _unmanagedBockDataArray[index] = blockData;

            // Update tile flag
            unmanagedTileData.flag |= TileFlag.Occupied;
            switch (blockData.blockType)
            {
                case BlockType.Key:
                    unmanagedTileData.flag |= TileFlag.HasKey;
                    break;
                case BlockType.Locked:
                    unmanagedTileData.flag |= TileFlag.Locked;
                    break;
            }

            _unmanagedTileDataArray[index] = unmanagedTileData;
            
            return true;
        }

        public bool IsPositionOccupied(MatrixPosition position)
        {
            var index = position.ToIndex(_columns);
            if (index < 0 || index >= _unmanagedBockDataArray.Length) return true;
            
            
            var unmanagedTileData = _unmanagedTileDataArray[index];
            return (unmanagedTileData.flag & TileFlag.Occupied) != 0;
        }

        public int CountAvailableSpaces()
        {
            var count = 0;
            for (var i = 0; i < _unmanagedTileDataArray.Length; i++)
            {
                if((_unmanagedTileDataArray[i].flag & TileFlag.Occupied) == 0)
                    count++;
            }
            
            return count;
        }

        private void ProcessKeyUnlocks()
        {
            var availableKeys = _keyCountArray[0];

            for (var index = 0; index < _unmanagedBockDataArray.Length; index++)
            {
                var unmanagedBlockData = _unmanagedBockDataArray[index];
                if (unmanagedBlockData.blockType != BlockType.Locked || unmanagedBlockData.requiredKeys <= 0)
                {
                    continue;
                }
                
                var keysToUse = math.min(availableKeys, unmanagedBlockData.requiredKeys);
                unmanagedBlockData.requiredKeys -= keysToUse;
                availableKeys -= keysToUse;
                    
                _unmanagedBockDataArray[index] = unmanagedBlockData;

                if (unmanagedBlockData.requiredKeys == 0)
                {
                    var unmanagedTileData = _unmanagedTileDataArray[index];
                    unmanagedTileData.flag &= ~TileFlag.Locked;
                    _unmanagedTileDataArray[index] = unmanagedTileData;
                }
                    
                if(availableKeys == 0) break;
            }
            
            _keyCountArray[0] = availableKeys;
        }
        
        public void Dispose()
        {
            if(_unmanagedBockDataArray.IsCreated) _unmanagedBockDataArray.Dispose();
            if(_unmanagedTileDataArray.IsCreated) _unmanagedTileDataArray.Dispose();
            if(_keyCountArray.IsCreated) _keyCountArray.Dispose();
        }
    }
}

