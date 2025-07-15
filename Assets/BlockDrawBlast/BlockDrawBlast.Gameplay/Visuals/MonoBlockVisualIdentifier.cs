using EncosyTower.StringIds;
using EncosyTower.UnityExtensions;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public sealed class MonoBlockVisualIdentifier
    {
        public string AssetKey { get; internal set; }
        
        public StringId KeyId { get; internal set; }
        
        public UnityInstanceId<GameObject> GameObjectId { get; internal set; }
        
        public Transform Transform { get; internal set; }
        
        public GameObject GameObject { get; internal set; }
        
        public MonoBlockVisual Visual { get; internal set; }
    }
}

