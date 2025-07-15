using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EncosyTower.Logging;
using EncosyTower.UnityExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace BlockDrawBlast.Gameplay
{
    public enum InputEventType
    {
        TouchStart,
        TouchMove,
        TouchEnd,
        Click
    }
    
    public struct InputEvent
    {
        public InputEventType eventType;
        public float2 screenPosition;
        public float2 deltaPosition;
        public float3 worldPosition;
        public float timestamp;
        public float duration;
    }
    
    public class MonoInputReceiver : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private float _clickTimeThreshold = 0.3f;
        [SerializeField] private float _clickDistanceThreshold = 10f;
        [SerializeField] private float _worldPlaneDistance = 10f;
        
        private Touchscreen _touchscreen;
        private Mouse _mouse;
        private TouchInputData _currentTouchData;
        private readonly Queue<InputEvent> _inputEventQueue = new();
        private bool _isInitialized;

        // Simple events - chỉ cần basic input
        public event Action<InputEvent> OnInputEvent;
        public event Action<float3> OnTouchStart;    // World position
        public event Action<float3> OnTouchMove;     // World position  
        public event Action<float3> OnTouchEnd;      // World position
        public event Action<float3> OnClick;         // World position

        private void Awake()
        {
            InitializeInput();
        }

        private void OnEnable()
        {
            if (_isInitialized)
            {
                EnhancedTouchSupport.Enable();
            }
        }
        
        private void OnDisable()
        {
            if (_isInitialized)
            {
                EnhancedTouchSupport.Disable();
            }
        }

        private void InitializeInput()
        {
            _touchscreen = Touchscreen.current;
            _mouse = Mouse.current;

            if (_mainCamera.IsInvalid())
            {
                _mainCamera = Camera.main;
            }
            
            EnhancedTouchSupport.Enable();
            _isInitialized = true;
        }

        private void Update()
        {
            ProcessTouchInput();
            ProcessMouseInputAsFallback();
        }

        private void ProcessTouchInput()
        {
            if(_touchscreen == null) return;
            
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

            if (activeTouches.Count > 0)
            {
                var primaryTouch = activeTouches[0];
                ProcessTouch(primaryTouch);
            }
            else
            {
                if (_currentTouchData.isPressed)
                {
                    HandleTouchEnd();
                }
            }
        }

        private void ProcessMouseInputAsFallback()
        {
            if (_touchscreen is not null
                && UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
            {
                return;
            }
            
            if(_mouse == null) return;
            
            var mousePosition = _mouse.position.ReadValue();
            var mousePressed = _mouse.leftButton.isPressed;
            var mouseJustPressed = _mouse.leftButton.wasPressedThisFrame;
            var mouseJustReleased = _mouse.leftButton.wasReleasedThisFrame;

            if (mouseJustPressed)
            {
                HandleTouchStart(new float2(mousePosition.x, mousePosition.y));
            }
            else if (mousePressed)
            {
                HandleTouchMove(new float2(mousePosition.x, mousePosition.y));
            } 
            else if (mouseJustReleased)
            {
                HandleTouchEnd();
            }
        }

        private void ProcessTouch(UnityEngine.InputSystem.EnhancedTouch.Touch touch)
        {
            var touchPosition = new float2(touch.screenPosition.x, touch.screenPosition.y);

            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    HandleTouchStart(touchPosition);
                    break;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                    HandleTouchMove(touchPosition);
                    break;
                
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    HandleTouchEnd();
                    break;

                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    _currentTouchData.position = touchPosition;
                    break;
            }
        }

        private void HandleTouchStart(float2 screenPosition)
        {
            _currentTouchData = new TouchInputData()
            {
                position = screenPosition,
                deltaPosition = float2.zero,
                isPressed = true,
                pressStartTime = Time.time,
                pressStartPosition = screenPosition
            };
            
            ProcessTouchStart(screenPosition);
        }

        private void HandleTouchMove(float2 screenPosition)
        {
            if(_currentTouchData.isPressed == false) return;
            
            var deltaPosition = screenPosition - _currentTouchData.position;
            _currentTouchData.position = screenPosition;
            _currentTouchData.deltaPosition = deltaPosition;

            ProcessTouchMove(screenPosition);
        }

        private void HandleTouchEnd()
        {
            if(_currentTouchData.isPressed == false) return;
            
            var screenPosition = _currentTouchData.position;
            var pressDuration = _currentTouchData.PressDuration;
            var dragDistance = _currentTouchData.DragDistanceLength;
            
            _currentTouchData.isPressed = false;

            // Check if it was a click
            if (pressDuration <= _clickTimeThreshold && dragDistance <= _clickDistanceThreshold)
            {
                ProcessClick(screenPosition);
            }
            
            ProcessTouchEnd(screenPosition);
        }

        private void ProcessTouchStart(float2 screenPosition)
        {
            var worldPosition = ScreenToWorldPosition(screenPosition);
            var inputEvent = CreateInputEvent(InputEventType.TouchStart, screenPosition, float2.zero);
            EnqueueInputEvent(inputEvent);
            
            OnTouchStart?.Invoke(worldPosition);
        }

        private void ProcessTouchMove(float2 screenPosition)
        {
            var worldPosition = ScreenToWorldPosition(screenPosition);
            var inputEvent = CreateInputEvent(InputEventType.TouchMove, screenPosition, _currentTouchData.deltaPosition);
            EnqueueInputEvent(inputEvent);
            
            OnTouchMove?.Invoke(worldPosition);
        }
        
        private void ProcessTouchEnd(float2 screenPosition)
        {
            var worldPosition = ScreenToWorldPosition(screenPosition);
            var inputEvent = CreateInputEvent(InputEventType.TouchEnd, screenPosition, float2.zero);
            inputEvent.duration = _currentTouchData.PressDuration;
            EnqueueInputEvent(inputEvent);
            
            OnTouchEnd?.Invoke(worldPosition);
        }
        
        private void ProcessClick(float2 screenPosition)
        {
            var worldPosition = ScreenToWorldPosition(screenPosition);
            var inputEvent = CreateInputEvent(InputEventType.Click, screenPosition, float2.zero);
            inputEvent.duration = _currentTouchData.PressDuration;
            EnqueueInputEvent(inputEvent);
            
            OnClick?.Invoke(worldPosition);
        }
        
        private float3 ScreenToWorldPosition(float2 screenPosition)
        {
            if (_mainCamera.IsInvalid())
            {
                return new float3(screenPosition.x, screenPosition.y, 0f);
            }
            
            var distanceToPlane = math.abs(_mainCamera.transform.position.z - _worldPlaneDistance);
            var screenPos = new Vector3(screenPosition.x, screenPosition.y, distanceToPlane);
            var worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
    
            return RoundToDecimalPlaces(worldPos, 3);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 RoundToDecimalPlaces(Vector3 value, int decimalPlaces)
        {
            float multiplier = math.pow(10f, decimalPlaces);
            return new float3(
                math.round(value.x * multiplier) / multiplier,
                math.round(value.y * multiplier) / multiplier,
                math.round(value.z * multiplier) / multiplier
            );
        }

        
        private InputEvent CreateInputEvent(InputEventType eventType, float2 screenPosition, float2 deltaPosition)
        {
            var inputEvent = new InputEvent
            {
                eventType = eventType,
                screenPosition = screenPosition,
                deltaPosition = deltaPosition,
                worldPosition = ScreenToWorldPosition(screenPosition),
                timestamp = Time.time,
                duration = 0f
            };
                
            return inputEvent;
        }
        
        private void EnqueueInputEvent(InputEvent inputEvent)
        {
            _inputEventQueue.Enqueue(inputEvent);
            OnInputEvent?.Invoke(inputEvent);
        }
        
        // Utility methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeueInputEvent(out InputEvent inputEvent)
        {
            return _inputEventQueue.TryDequeue(out inputEvent);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TouchInputData GetCurrentTouchData()
        {
            return _currentTouchData;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPressed()
        {
            return _currentTouchData.isPressed;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3 GetTouchWorldPosition()
        {
            return ScreenToWorldPosition(_currentTouchData.position);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2 GetTouchScreenPosition()
        {
            return _currentTouchData.position;
        }
        
        public void ClearInputQueue()
        {
            _inputEventQueue.Clear();
        }
    }
}