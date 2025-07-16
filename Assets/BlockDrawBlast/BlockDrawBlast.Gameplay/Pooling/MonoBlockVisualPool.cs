using System;
using System.Collections.Generic;
using EncosyTower.Collections;
using EncosyTower.Common;
using EncosyTower.Logging;
using EncosyTower.Pooling;
using EncosyTower.StringIds;
using EncosyTower.UnityExtensions;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoBlockVisualPool : IDisposable
    {
        private readonly GameObjectPool _pool = new();
        private readonly HashSet<UnityInstanceId<GameObject>> _instanceIds = new();
        private readonly Transform _poolParent;
        private readonly Transform _activeParent;
        
        public MonoBlockVisualPool(Transform poolParent, Transform activeParent)
        {
            _poolParent = poolParent;
            _activeParent = activeParent;
        }

        public int PoolingCount => _pool.UnusedCount;
        public bool IsInitialized => _pool.Prefab != null;
        
        public void Initialize(GameObject source)
        {
            if(IsInitialized) return;

            _pool.Prefab = new GameObjectPrefab()
            {
                Parent = _poolParent,
                Source = source,
                InstantiateInWorldSpace = false
            };
        }

        public void Prepool(int amount)
        {
            if(_pool.Prepool(amount) == false) return;
            
            _instanceIds.EnsureCapacity(amount);
        }

        public Option<MonoBlockVisualIdentifier> GetBlockVisualFromPool(StringId keyId)
        {
            var gameObject = _pool.RentGameObject(false);
            var findResult = gameObject.TryGetComponent<IMonoBlockVisual>(out var blockVisual);

            if (findResult == false || blockVisual is not Component component)
            {
                UnityEngine.Object.Destroy(gameObject);
                return default;
            }

            var transform = component.transform;
            var identifier = gameObject.GetOrAddComponent<MonoBlockVisualIdentifier>();
            
            identifier.KeyId = keyId;
            identifier.GameObjectId = gameObject;
            identifier.Transform = transform;
            identifier.Transform.SetParent(_activeParent, false);
            identifier.GameObject = gameObject;
            identifier.MonoBlockVisual = blockVisual;

            if (_instanceIds.Add(identifier.GameObjectId) == false)
            {
                DevLoggerAPI.LogWarning(" ");
            }
            
            return identifier;
        }
        
        public void ReturnBlockVisualToPool(MonoBlockVisualIdentifier identifier)
        {
            if (_instanceIds.Remove(identifier.GameObjectId) == false)
            {
                DevLoggerAPI.LogWarning(" ");
            }
            
            identifier.Transform.SetParent(_poolParent, false);
            
            _pool.Return(identifier.GameObject);
        }
        
        public void Dispose()
        {
            ReturnActives();

            _pool.ReleaseInstances(0);
            _pool.Dispose();
        }

        public void Destroy(int amountToDestroy)
        {
            _pool.ReleaseInstances(_pool.UnusedCount - amountToDestroy);
        }

        private void ReturnActives()
        {
            var length = _instanceIds.Count;
            var array = NativeArray.CreateFast<int>(length, Unity.Collections.Allocator.Temp);
            var index = 0;

            foreach (var instanceId in _instanceIds)
            {
                array[index] = (int)instanceId;
                index++;
            }

            _instanceIds.Clear();
            _pool.ReturnInstanceIds(array);
        }
    }
}

