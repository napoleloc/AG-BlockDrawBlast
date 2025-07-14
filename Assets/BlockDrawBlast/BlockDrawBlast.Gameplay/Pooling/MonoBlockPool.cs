using System;
using System.Collections.Generic;
using EncosyTower.Pooling;
using EncosyTower.UnityExtensions;
using UnityEngine;

namespace BlockDrawBlast.Gameplay.Pooling
{
    public class MonoBlockPool : IDisposable
    {
        private readonly GameObjectPool _pool = new();
        private readonly HashSet<UnityInstanceId<GameObject>> _instanceIds = new();
        private readonly Transform _poolParent;
        private readonly Transform _activeParent;

        public MonoBlockPool(Transform poolParent, Transform activeParent)
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
        
        public void Dispose()
        {
            
        }
    }
}

