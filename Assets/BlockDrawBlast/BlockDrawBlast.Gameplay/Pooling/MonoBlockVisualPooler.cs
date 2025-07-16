using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using EncosyTower.AddressableKeys;
using EncosyTower.Common;
using EncosyTower.Ids;
using EncosyTower.Logging;
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

        private void Awake()
        {
            GlobalObjectVault.TryAdd(TypeId, this);
        }

        private void OnDestroy()
        {
            GlobalObjectVault.TryRemove(TypeId, out _);
        }

        public UniTask<Option<MonoBlockVisualIdentifier>> GetBlockVisualFromPoolAsync(string assetKey, BlockData blockData, CancellationToken token)
        {
            var key = MakeBlockVisualKey(assetKey);
            return GetBlockVisualFromPoolAsync(key, blockData, token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BlockVisualKey MakeBlockVisualKey(string srt)
        {
            var id = StringToId.MakeFromUnmanaged(srt);
            return new BlockVisualKey(srt, id);
        }
        
        internal async UniTask<Option<MonoBlockVisualIdentifier>> GetBlockVisualFromPoolAsync(BlockVisualKey key, BlockData blockData, CancellationToken token)
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

            var identifierOpt = pool.GetBlockVisualFromPool(key.Id);

            if (identifierOpt.TryValue(out var identifier) && identifier.MonoBlockVisual is IMonoBlockVisualCreateAsync createAsync)
            {
                await createAsync.OnCreateAsync(blockData, token);

                if (token.IsCancellationRequested)
                {
                    ReturnBlockVisualToPool(identifier);
                    return default;
                }
            }
            
            return identifierOpt;
        }

        public bool ReturnBlockVisualToPool(IMonoBlockVisual blockVisual)
        {
            if (blockVisual is not Component component)
            {
                return false;
            }

            if (component.TryGetComponent(out MonoBlockVisualIdentifier identifier) == false)
            {
                DevLoggerAPI.LogError("BlockVisualIdentifier is not found.");
                return false;
            }
            
            return ReturnBlockVisualToPool(identifier);
        }
        
        public bool ReturnBlockVisualToPool(MonoBlockVisualIdentifier identifier)
        {
            if(_idToPool.TryGetValue(identifier.KeyId, out var pool) == false)
            {
                return false;
            }
            
            pool.ReturnBlockVisualToPool(identifier);
            return true;
        }

        private MonoBlockVisualPool GetPoolFor(BlockVisualKey key)
        {
            if (_idToPool.TryGetValue(key.Id, out var pool) == false)
            {
                _idToPool[key.Id] = pool = new MonoBlockVisualPool(transform, transform);
            }
            
            return pool;
        }

        internal readonly record struct BlockVisualKey(string Value, StringId Id);
    }
}

