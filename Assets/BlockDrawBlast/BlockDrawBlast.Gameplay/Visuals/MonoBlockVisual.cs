using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoBlockVisual : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        public void BindToMaterial(Material material)
        {
            _renderer.material = material;
        }
    }
}

