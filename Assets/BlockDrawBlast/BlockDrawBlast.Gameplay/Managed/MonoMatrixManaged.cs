using System;
using System.Runtime.CompilerServices;
using EncosyTower.Ids;
using EncosyTower.Logging;
using EncosyTower.Types;
using EncosyTower.Vaults;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoMatrixManaged : MonoBehaviour, IDisposable
    {
        public static readonly Id<MonoMatrixManaged> TypeId = Type<MonoMatrixManaged>.Id;
        
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        
        private MonoMatrixVisual _monoMatrixVisual;
        
        private NativeArray<BlockData> _unmanagedBockDataArray;
        private NativeArray<TileData> _unmanagedTileDataArray;
        private NativeArray<int> _lockedBlockIndices;
        private NativeArray<int> _availableKeys;
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
            Dispose();
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
            if(_lockedBlockIndices.IsCreated) _lockedBlockIndices.Dispose();
            if(_availableKeys.IsCreated) _availableKeys.Dispose();
            
            _unmanagedBockDataArray = new NativeArray<BlockData>(initialCapacity, Allocator.Persistent);
            _unmanagedTileDataArray = new NativeArray<TileData>(initialCapacity, Allocator.Persistent);
         
            _availableKeys = new NativeArray<int>(1, Allocator.Persistent);
            
            preparedTileDataArray.CopyTo(_unmanagedTileDataArray);
            var blockCount = preparedTileDataArray.Length;
            
            _lockedBlockIndices = new NativeArray<int>(blockCount , Allocator.Persistent);
            
            for (int i = 0; i < blockCount ; i++)
            {
                var preparedBlockData = preparedBlockDataArray[i];
                var position = preparedBlockData.position;
                var index = position.ToIndex(_columns);
                
                if (index < 0 || index >= _unmanagedBockDataArray.Length)
                {
                    DevLoggerAPI.LogError($"Invalid index: {index}");
                    continue;
                }

                if ((preparedBlockData.blockFlag & BlockFlag.Locked) == 0)
                {
                    _lockedBlockIndices[i] = index;
                }
                
                _unmanagedBockDataArray[index] = preparedBlockData;
                _monoMatrixVisual.CreateAsync(position, preparedBlockData);
            }
            
            _isInitialized = true;
        }
        
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _unmanagedBockDataArray.Length;
        }


        public unsafe bool TryPlaceBlock(MatrixPosition position, DrawingBlockContext context)
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

            // Get a pointer to an element
            ref var unmanagedBlockDataRef = ref UnsafeUtility.ArrayElementAsRef<BlockData>(
                _unmanagedBockDataArray.GetUnsafePtr(), index);

            unmanagedBlockDataRef.blockFlag |= BlockFlag.Normal;
            unmanagedBlockDataRef.colorType = context.colorType;
            unmanagedBlockDataRef.position = context.position;
    
            // Update tile flag
            unmanagedTileData.flag |= TileFlag.Occupied;
            _unmanagedTileDataArray[index] = unmanagedTileData;
            
            _monoMatrixVisual.CreateAsync(position, unmanagedBlockDataRef);
            
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
            var length = _unmanagedBockDataArray.Length;
            
            for (var i = 0; i < length; i++)
            {
                if((_unmanagedTileDataArray[i].flag & TileFlag.Occupied) == 0)
                    count++;
            }
            
            return count;
        }

        private unsafe void ProcessKeyUnlocks()
        {
            var availableKeys = _availableKeys[0];
            var length = _unmanagedBockDataArray.Length;

            for (int index = 0; index < _lockedBlockIndices.Length; index++)
            {
                var indexToLockedBlock = _lockedBlockIndices[index];
                if (indexToLockedBlock < 0 || indexToLockedBlock >= _unmanagedBockDataArray.Length)
                {
                    DevLoggerAPI.LogError($"Invalid index: {indexToLockedBlock}");
                    continue;
                }
                
                // Get a pointer to an element
                ref var unmanagedBlockDataRef = ref UnsafeUtility.ArrayElementAsRef<BlockData>(
                    _unmanagedBockDataArray.GetUnsafePtr(), indexToLockedBlock);
                
                if (unmanagedBlockDataRef.blockFlag != BlockFlag.Locked || unmanagedBlockDataRef.requiredKeys <= 0)
                {
                    continue;
                }
        
                var keysToUse = math.min(availableKeys, unmanagedBlockDataRef.requiredKeys);
                unmanagedBlockDataRef.requiredKeys -= keysToUse;
                availableKeys -= keysToUse;
            
                if (unmanagedBlockDataRef.requiredKeys == 0)
                {
                    unmanagedBlockDataRef.blockFlag &= ~BlockFlag.Locked;
                }
            }
            
            _availableKeys[0] = availableKeys;
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidPosition(MatrixPosition position)
        {
            return position.RowIndex >= 0 && position.RowIndex < _rows 
                                          && position.ColumnIndex >= 0 && position.ColumnIndex < _columns;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MatrixPosition ClampPosition(MatrixPosition position)
        {
            return new MatrixPosition
            {
                RowIndex = math.clamp(position.RowIndex, 0, _rows - 1),
                ColumnIndex = math.clamp(position.ColumnIndex, 0, _columns - 1)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int2 ClampPosition(int2 position)
        {
            return new int2(
                math.clamp(position.x, 0, _columns - 1),
                math.clamp(position.y, 0, _rows - 1)
            );
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
            if(_availableKeys.IsCreated) _availableKeys.Dispose();
        }
    }
}

