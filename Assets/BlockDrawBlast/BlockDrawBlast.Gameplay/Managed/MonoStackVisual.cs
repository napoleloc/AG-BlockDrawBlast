using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoStackVisual : MonoBehaviour
    {
        [Title("Animations", titleAlignment: TitleAlignments.Centered)] 
        [SerializeField] private float _animationDuration;
        [SerializeField] private Ease _animationEase;
        [SerializeField] private float _consumeAnimationDuration;
        
        [Title("Current Stack Indicator", titleAlignment: TitleAlignments.Centered)] 
        [SerializeField] private Transform _currentStackIndicator;
        [SerializeField] private Color _currentStackColor = Color.yellow;
        [SerializeField] private float _indicatorPulseSpeed = 2F;
    }
}

