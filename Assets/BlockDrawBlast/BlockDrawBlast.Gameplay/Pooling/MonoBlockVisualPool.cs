using System;
using System.Collections.Generic;
using EncosyTower.Collections;
using EncosyTower.Pooling;
using EncosyTower.UnityExtensions;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoBlockVisualPool : IDisposable
    {
        private readonly GameObjectPool _pool = new();
        private readonly HashSet<UnityInstanceId<GameObject>> _instanceIds = new();
        private readonly FasterList<MonoBlockVisualIdentifier> _blockVisualIds;
        private readonly Transform _poolParent;
        private readonly Transform _activeParent;
        
        public MonoBlockVisualPool(Transform poolParent, Transform activeParent, FasterList<MonoBlockVisualIdentifier> blockVisualIds)
        {
            _poolParent = poolParent;
            _activeParent = activeParent;
            _blockVisualIds = blockVisualIds;
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
            _blockVisualIds.IncreaseCapacityTo(amount);
        }
        
        public void Dispose()
        {
            
            
        }
    }
}

