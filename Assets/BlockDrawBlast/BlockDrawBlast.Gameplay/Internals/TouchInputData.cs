using Unity.Mathematics;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public struct TouchInputData
    {
        public float2 position;
        public float2 deltaPosition;
        public bool isPressed;
        public float pressStartTime;
        public float2 pressStartPosition;
        
        public bool IsDragging => isPressed && math.lengthsq(deltaPosition) > 0.01f;
        public float PressDuration => isPressed ? Time.time - pressStartTime : 0f;
        
        public float DragDistanceLength => math.length(position - pressStartPosition);
    }
}

