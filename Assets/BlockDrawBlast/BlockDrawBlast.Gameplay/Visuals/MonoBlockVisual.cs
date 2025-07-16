using EncosyTower.UnityExtensions;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoBlockVisual : MonoBehaviour, IMonoBlockVisual
        , IMonoBlockVisualHasEffect
    {
        [SerializeField] private Renderer _renderer;

        private MonoBlockVisualEffect _monoBlockVisualEffect;

        public IMonoBlockVisualEffect MonoBlockVisualEffect
        {
            get
            {
                if (_monoBlockVisualEffect.IsInvalid())
                {
                    _monoBlockVisualEffect = GetComponent<MonoBlockVisualEffect>();
                }
                
                return _monoBlockVisualEffect;
            }
        }
        
        public void BindToMaterial(Material material)
        {
            _renderer.material = material;
        }
    }
}

