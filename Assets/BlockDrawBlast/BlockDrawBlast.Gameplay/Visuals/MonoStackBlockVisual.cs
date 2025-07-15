using LitMotion;
using LitMotion.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoStackBlockVisual : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private TMP_Text _labelCount;
        
        [Title("Animations", titleAlignment: TitleAlignments.Centered)]
        [SerializeField] private float _entranceAnimationDuration = 0.3f;
        [SerializeField] private float _consumeAnimationDuration = 0.2f;
        [SerializeField] private Ease _entranceEase = Ease.OutBounce;
        
        private int _count;

        public void Initialize(int count, Material material)
        {
            _count = count;
            
            _renderer.material = material;
            _labelCount.text = _count.ToString();
            
            AnimationCountUpdate();
            
            transform.localScale = Vector3.zero;
        }

        public void UpdateCount(int newCount)
        {
            _count = newCount;
            
            _labelCount.text = _count.ToString();
        }

        private void AnimationCountUpdate()
        {
            var originalScale = _labelCount.transform.localScale;
            var targetScale = originalScale * 1.3f;
                
            LMotion.Create(originalScale, targetScale, 0.15f)
                .WithEase(Ease.OutQuart)
                .WithLoops(2, LoopType.Yoyo)
                .BindToLocalScale(_labelCount.transform);
        }
    }
}

