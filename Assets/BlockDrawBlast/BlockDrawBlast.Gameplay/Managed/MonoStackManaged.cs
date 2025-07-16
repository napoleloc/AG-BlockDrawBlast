using System;
using System.Runtime.CompilerServices;
using EncosyTower.Logging;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoStackManaged : MonoBehaviour
    {
        [SerializeField] private MonoStackVisual _monoStackVisual;

        private NativeArray<StackBlockData> _unmanagedDataArray;

        private int _currentBlockIndex;
        
        public void PreparedWhenStartGame(ReadOnlySpan<StackBlockData> dataSpan)
        {
            var capacity = dataSpan.Length;

            if (capacity < 1)
            {
                DevLoggerAPI.LogError(" ");
                return;
            }
            
            // Dispose previous data if exists
            if (_unmanagedDataArray.IsCreated)
            {
                _unmanagedDataArray.Dispose();
            }
            
            _unmanagedDataArray = new NativeArray<StackBlockData>(capacity, Allocator.Persistent);
            dataSpan.CopyTo(_unmanagedDataArray);
        }
        
        public bool TryConsumeOneFromCurrentBlock()
        {
            if(_currentBlockIndex >= _unmanagedDataArray.Length) return false;
            
            
            var currentBlockData = _unmanagedDataArray[_currentBlockIndex];
            
            if(currentBlockData.count <= 0) return false;
            
            currentBlockData.count--;
            _unmanagedDataArray[_currentBlockIndex] = currentBlockData;
            
            _monoStackVisual.UpdateCurrentBlockCount(currentBlockData.count);
            
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetCurrentBlock(out StackBlockData result)
        {
            if (_currentBlockIndex < _unmanagedDataArray.Length)
            {
                result = _unmanagedDataArray[_currentBlockIndex];
                return true;
            }
            
            result = default;
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetNextBlock(out StackBlockData result)
        {
            var nextBlockIndex = _currentBlockIndex + 1;
            if (nextBlockIndex < _unmanagedDataArray.Length)
            {
                result = _unmanagedDataArray[nextBlockIndex];
                return true;
            }
            
            result = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCurrentBlockCount()
        {
            return TryGetCurrentBlock(out var result) == false ? 0 : result.count;
        }

        public void MoveToNextBlock()
        {
            _currentBlockIndex++;

            if (_currentBlockIndex >= _unmanagedDataArray.Length)
            {
                
                return;
            }
            
            UpdateStackVisual();
        }

        private void UpdateStackVisual()
        {
            var remain = math.clamp(_unmanagedDataArray.Length - _currentBlockIndex, 0, 2);
            
            switch (remain)
            {
                case 0:
                {
                    break;
                }
                case 1:
                {
                    var currentBlockData = _unmanagedDataArray[_currentBlockIndex];
                    
                    break;
                }

                default:
                {
                    var currentBlockData = _unmanagedDataArray[_currentBlockIndex];
                    var nextBlockData = _unmanagedDataArray[_currentBlockIndex + 1];
                    
                    break;
                }
            }
        }

        public void Dispose()
        {
            if(_unmanagedDataArray.IsCreated) _unmanagedDataArray.Dispose();
        }
    }
}

