using EncosyTower.Ids;
using EncosyTower.Types;
using EncosyTower.Vaults;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoWorldCamera : MonoBehaviour
    {
        public static readonly Id<MonoWorldCamera> TypeId = Type<MonoWorldCamera>.Id;

        private void Awake()
        {
            GlobalObjectVault.TryAdd(TypeId, this);
        }

        private void OnDestroy()
        {
            GlobalObjectVault.TryRemove(TypeId, out _);
        }
    }
}

