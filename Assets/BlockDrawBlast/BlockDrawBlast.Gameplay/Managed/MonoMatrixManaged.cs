using System;
using System.Runtime.CompilerServices;
using System.Threading;
using EncosyTower.Ids;
using EncosyTower.Logging;
using EncosyTower.Types;
using EncosyTower.UnityExtensions;
using EncosyTower.Vaults;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoMatrixManaged : MonoBehaviour, IDisposable
    {
        public static readonly Id<MonoMatrixManaged> TypeId = Type<MonoMatrixManaged>.Id;
        
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        [SerializeField] private MonoMatrixVisual _monoMatrixVisual;
        
        private NativeArray<BlockData> _unmanagedBockDataArray;
        private NativeArray<TileData> _unmanagedTileDataArray;
        private NativeArray<int> _keyCountArray;
        private float3 _originPosition;
        
        private bool _isInitialized;

        private async void Awake()
        {
            _monoMatrixVisual = GetComponent<MonoMatrixVisual>();
            
            GlobalObjectVault.TryAdd(TypeId, this);
        }

        private void OnDestroy()
        {
            GlobalObjectVault.TryRemove(TypeId, out _);
        }

        public void PreparedWhenStartGame(
            int rows
            , int columns
            , ReadOnlySpan<TileData> preparedTileDataArray
            , ReadOnlySpan<BlockData> preparedBlockDataArray)
        {
            _rows = rows;
            _columns = columns;
            _originPosition = CalculateOriginPosition(rows, columns);
            
            var initialCapacity = rows * columns;
            
            if(_unmanagedBockDataArray.IsCreated) _unmanagedBockDataArray.Dispose();
            if (_unmanagedTileDataArray.IsCreated) _unmanagedTileDataArray.Dispose();
            if(_keyCountArray.IsCreated) _keyCountArray.Dispose();
            
            _unmanagedBockDataArray = new NativeArray<BlockData>(initialCapacity, Allocator.Persistent);
            _unmanagedTileDataArray = new NativeArray<TileData>(initialCapacity, Allocator.Persistent);
            _keyCountArray = new NativeArray<int>(1, Allocator.Persistent);
            
            preparedTileDataArray.CopyTo(_unmanagedTileDataArray);

            for (int i = 0; i < preparedBlockDataArray.Length; i++)
            {
                var preparedBlockData = preparedBlockDataArray[i];
                var position = preparedBlockData.position;
                var index = position.ToIndex(_columns);
                
                if (index < 0 || index >= _unmanagedBockDataArray.Length)
                {
                    DevLoggerAPI.LogError($"Invalid index: {index}");
                    continue;
                }
                
                _unmanagedBockDataArray[index] = preparedBlockData;
                _monoMatrixVisual.CreateAsync(position, preparedBlockData);
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
            _unmanagedTileDataArray[index] = unmanagedTileData;
            
            _monoMatrixVisual.CreateAsync(position, blockData);
            
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
            var length = _unmanagedBockDataArray.Length;
            
            var unmanagedBlockDataCopy = new NativeArray<BlockData>(length, Allocator.Temp);
            _unmanagedBockDataArray.CopyTo(unmanagedBlockDataCopy);
            
            var unmanagedTileDataCopy = new NativeArray<TileData>(length, Allocator.Temp);
            _unmanagedTileDataArray.CopyTo(unmanagedTileDataCopy);

            for (var index = 0; index < length; index++)
            {
                // Get a pointer to element
                // ref var unmanagedBlockData = ref UnsafeUtility.ArrayElementAsRef<BlockData>(
                //     _unmanagedBockDataArray.GetUnsafePtr(), index);
                
                var unmanagedBlockData = _unmanagedBockDataArray[index];
                if (unmanagedBlockData.blockFlag != BlockFlag.Locked || unmanagedBlockData.requiredKeys <= 0)
                {
                    continue;
                }
                
                var keysToUse = math.min(availableKeys, unmanagedBlockData.requiredKeys);
                unmanagedBlockData.requiredKeys -= keysToUse;
                availableKeys -= keysToUse;
                    
                if (unmanagedBlockData.requiredKeys == 0)
                {
                    var unmanagedTileData = _unmanagedTileDataArray[index];
                    unmanagedTileData.flag &= ~TileFlag.Locked;
                    _unmanagedTileDataArray[index] = unmanagedTileData;
                    
                    unmanagedBlockData.blockFlag &= ~BlockFlag.Locked;
                }
                
                _unmanagedBockDataArray[index] = unmanagedBlockData;
                    
                if(availableKeys == 0) break;
            }
            
            _keyCountArray[0] = availableKeys;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MatrixPosition WorldToMatrixPosition(float3 worldPosition)
        {
            // Implementation depends on your grid system
            var row = (worldPosition - _originPosition).y;
            var col = (worldPosition - _originPosition).x;
            
            return new MatrixPosition((int)math.round(-row), (int)math.round(col));
        }
        
        private float3 CalculateOriginPosition(int rowCount, int columnCount)
        {
            var offsetY = Mathf.Floor(rowCount / 2.0f);
            var offsetX = Mathf.Floor(columnCount / 2.0f);

            return new float3(-offsetX, offsetY, 0);
        }
        
        public void Dispose()
        {
            if(_unmanagedBockDataArray.IsCreated) _unmanagedBockDataArray.Dispose();
            if(_unmanagedTileDataArray.IsCreated) _unmanagedTileDataArray.Dispose();
            if(_keyCountArray.IsCreated) _keyCountArray.Dispose();
        }
    }
}

