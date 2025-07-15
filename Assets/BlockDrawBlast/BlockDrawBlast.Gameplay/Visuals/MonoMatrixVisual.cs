using System;
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
        
        private readonly Dictionary<MatrixPosition, MonoBlockVisual> _coordToBlockVisual = new();

        private void Awake()
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask CreateAsync(MatrixPosition position, BlockData blockData, CancellationToken token)
        {
            return CreateAsyncInternal(position, blockData, token);
        }
        
        private async UniTask CreateAsyncInternal(MatrixPosition position, BlockData blockData, CancellationToken token)
        {
            var assetKey = Constants.GetBlockPrefabPath(blockData.blockType);

            if (string.IsNullOrEmpty(assetKey))
            {
                DevLoggerAPI.LogError("");
                return;
            }

            var identifierOpt = await _blockVisualPooler.GetBlockVisualAsync(assetKey, token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            if (identifierOpt.TryValue(out var identifier) == false)
            {
                return;
            }
            
            var visual = identifier.Visual;
            
            visual.BindToMaterial(default);
            _coordToBlockVisual.TryAdd(position, visual);
        }
        
        public void Release(MatrixPosition position)
        {
            if (_coordToBlockVisual.TryGetValue(position, out var visual) == false)
            {
                return;
            }
            
            visual.gameObject.SetActive(false);
            _coordToBlockVisual.Remove(position);
        }
    }
}

