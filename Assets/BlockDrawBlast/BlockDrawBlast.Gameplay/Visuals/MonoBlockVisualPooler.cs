using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using EncosyTower.AddressableKeys;
using EncosyTower.Collections;
using EncosyTower.Common;
using EncosyTower.Ids;
using EncosyTower.StringIds;
using EncosyTower.Types;
using EncosyTower.UnityExtensions;
using EncosyTower.Vaults;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoBlockVisualPooler : MonoBehaviour
    {
        public static readonly Id<MonoBlockVisualPooler> TypeId = Type<MonoBlockVisualPooler>.Id;
        
        private readonly Dictionary<StringId, MonoBlockVisualPool> _idToPool = new();
        private readonly FasterList<MonoBlockVisualIdentifier> _blockVisualIds = new();

        private void Awake()
        {
            GlobalObjectVault.TryAdd(TypeId, this);
        }

        private void OnDestroy()
        {
            GlobalObjectVault.TryRemove(TypeId, out _);
        }

        public UniTask<Option<MonoBlockVisualIdentifier>> GetBlockVisualAsync(string assetKey, CancellationToken token)
        {
            var key = MakeBlockVisualKey(assetKey);
            return GetBlockVisualAsync(key, token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BlockVisualKey MakeBlockVisualKey(string srt)
        {
            var id = StringToId.MakeFromUnmanaged(srt);
            return new BlockVisualKey(srt, id);
        }
        
        internal async UniTask<Option<MonoBlockVisualIdentifier>> GetBlockVisualAsync(BlockVisualKey key, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return default;
            }

            var pool = GetPoolFor(key);

            if (pool.IsInitialized == false)
            {
                var sourceOpt = await new AddressableKey(key.Value).TryLoadAsync<GameObject>(token);
                
                if(token.IsCancellationRequested)
                {
                    return default;
                }

                if (sourceOpt.TryValue(out var source) || source.IsInvalid())
                {
                    return default;
                }
                
                pool.Initialize(source);
            }
            
            return default;
        }

        private MonoBlockVisualPool GetPoolFor(BlockVisualKey key)
        {
            if (_idToPool.TryGetValue(key.Id, out var pool) == false)
            {
                _idToPool[key.Id] = pool = new MonoBlockVisualPool(transform, transform, _blockVisualIds);
            }
            
            return pool;
        }

        internal readonly record struct BlockVisualKey(string Value, StringId Id);
    }
}

