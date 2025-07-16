using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using EncosyTower.Logging;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoMatrixVisual : MonoBehaviour
    {
        [SerializeField] private MonoBlockVisualPooler _blockVisualPooler;
        
        private readonly Dictionary<MatrixPosition, IMonoBlockVisual> _coordToBlockVisual = new();

        private void Awake()
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask CreateAsync(MatrixPosition position, BlockData blockData)
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

