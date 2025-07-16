using EncosyTower.StringIds;
using EncosyTower.UnityExtensions;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoBlockVisualIdentifier : MonoBehaviour
    {
        public StringId KeyId { get; internal set; }
        public UnityInstanceId<GameObject> GameObjectId { get; internal set; }
        public Transform Transform { get; internal set; }
        public GameObject GameObject { get; internal set; }
        public IMonoBlockVisual MonoBlockVisual { get; internal set; }
    }
}

