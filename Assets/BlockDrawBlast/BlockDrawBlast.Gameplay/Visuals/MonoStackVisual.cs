using System;
using EncosyTower.Collections;
using EncosyTower.Logging;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoStackVisual : MonoBehaviour
    {
        [SerializeField] private MonoStackBlockVisual _prefab;
        [SerializeField] private Transform _parent;
        
        [Title("Animations", titleAlignment: TitleAlignments.Centered)] 
        [SerializeField] private float _animationDuration;
        [SerializeField] private Ease _animationEase;
        [SerializeField] private float _consumeAnimationDuration;
        
        [Title("Current Stack Indicator", titleAlignment: TitleAlignments.Centered)] 
        [SerializeField] private Transform _currentStackIndicator;
        [SerializeField] private float _indicatorPulseSpeed = 2F;

        private readonly FasterList<MonoStackBlockVisual> _visibleBlocks = new();
        
        public void UpdateCurrentBlockCount(int newCount)
        {
            if (_visibleBlocks.Count < 1)
            {
                return;
                DevLoggerAPI.LogError("No visible blocks");
            }
            
            _visibleBlocks[0].UpdateCount(newCount);
        }
        

        private void RefreshVisibleBlocks(ReadOnlySpan<StackBlockData> entries)
        {
            
        }
    }
}

