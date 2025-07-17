using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using EncosyTower.Logging;
using EncosyTower.Vaults;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoMatrixVisual : MonoBehaviour
    {
        [SerializeField] private MonoBlockVisualPooler _blockVisualPooler;
        
        private readonly Dictionary<MatrixPosition, IMonoBlockVisual> _coordToBlockVisual = new();

        private async void Awake()
        {
            await InitializeAsync();
        }

        private async UniTask InitializeAsync()
        {
            using var initCts = new CancellationTokenSource();
            var blockVisualPoolerOpt = await GlobalObjectVault
                .TryGetAsync(MonoBlockVisualPooler.TypeId, this, initCts.Token);
                
            if (blockVisualPoolerOpt.TryValue(out var blockVisualPooler) == false)
            {
                DevLoggerAPI.LogError("BlockVisualPooler is not found.");
                return;
            }
                
            _blockVisualPooler = blockVisualPooler;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask CreateAsync(in MatrixPosition position, in BlockData blockData)
        {
            return CreateAsyncInternal(position, blockData, default);
        }
        
        private async UniTask CreateAsyncInternal(MatrixPosition position, BlockData blockData, CancellationToken token)
        {
            var assetKey = Constants.GetBlockPrefabPath(blockData.blockFlag);

            if (string.IsNullOrEmpty(assetKey))
            {
                DevLoggerAPI.LogError("");
                return;
            }

            var identifierOpt = await _blockVisualPooler.GetBlockVisualFromPoolAsync(assetKey, blockData, token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            if (identifierOpt.TryValue(out var identifier) == false)
            {
                return;
            }
            
            var blockVisual = identifier.MonoBlockVisual;
            
            _coordToBlockVisual.TryAdd(position, blockVisual);
        }
    }
}

