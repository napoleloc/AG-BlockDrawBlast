using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoBlockVisualEffect : MonoBehaviour
        , IMonoBlockVisualSpawnEffectAsync
        , IMonoBlockVisualDestructEffectAsync
    {
        [SerializeField] private ParticleSystem _particleDestructedEffect;
        
        private MotionHandle _spawnEffectHandle;
        private MotionHandle _destructEffectHandle;
        
        public UniTask OnPlaySpawnAsync(CancellationToken token)
        {
            _spawnEffectHandle.TryCancel();

            _spawnEffectHandle = LMotion.Create(0.5F, 1F, 0.25F)
                .WithEase(Ease.OutBack)
                .Bind(this, (value, state) => state.transform.localScale = Vector3.one * value);
            
            return _spawnEffectHandle.ToUniTask(token);
        }

        public UniTask OnPlayDestructAsync(CancellationToken token)
        {
            _destructEffectHandle.TryCancel();
            
                        
            return _destructEffectHandle.ToUniTask(token);
        }
    }
}

