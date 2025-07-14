using System;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    public readonly record struct DrawingContext
    {
        public int2 GridSize { get; init; }
        public int2 CurrentPosition { get; init; }
        public int2 PreviousPosition { get; init; }
        public DrawingRule DrawingRule { get; init; }
        
         // Validation methods
        public bool IsValidDrawPosition(ReadOnlySpan<TileState> tileStates)
        {
            var index = GetTileIndex(CurrentPosition);
            if (index < 0 || index >= tileStates.Length)
                return false;
                
            return tileStates[index] == TileState.Empty;
        }
        
        public bool IsConnectedToPrevious()
        {
            if (DrawingRule.RequiresConnectedPath == false)
                return true;
                
            var distance = math.abs(CurrentPosition.x - PreviousPosition.x) + 
                          math.abs(CurrentPosition.y - PreviousPosition.y);
            
            if (DrawingRule.AllowDiagonalDrawing)
            {
                var maxComponent = math.max(math.abs(CurrentPosition.x - PreviousPosition.x),
                                           math.abs(CurrentPosition.y - PreviousPosition.y));
                return maxComponent == 1;
            }
            
            return distance == 1;
        }
        
        public bool IsValidDrawingPattern(ReadOnlySpan<int2> drawPath)
        {
            return DrawingRule.Pattern switch
            {
                DrawingPattern.Continuous => IsValidContinuousPath(drawPath),
                DrawingPattern.Line => IsValidLinePath(drawPath),
                DrawingPattern.Individual => true,
                DrawingPattern.Shape => IsValidShapePath(drawPath),
                _ => false
            };
        }
        
        private bool IsValidContinuousPath(ReadOnlySpan<int2> path)
        {
            for (int i = 1; i < path.Length; i++)
            {
                var prev = path[i - 1];
                var curr = path[i];
                
                if (IsPositionsConnected(prev, curr) == false)
                    return false;
            }
            
            return true;
        }
        
        private bool IsValidLinePath(ReadOnlySpan<int2> path)
        {
            if (path.Length < 2)
                return true;
                
            var direction = path[1] - path[0];
            var normalizedDir = math.normalize(new float2(direction.x, direction.y));
            
            for (int i = 2; i < path.Length; i++)
            {
                var currentDir = path[i] - path[i - 1];
                var currentNormalizedDir = math.normalize(new float2(currentDir.x, currentDir.y));
                
                if (math.abs(math.dot(normalizedDir, currentNormalizedDir)) < 0.9f)
                    return false;
            }
            
            return true;
        }
        
        private bool IsValidShapePath(ReadOnlySpan<int2> path)
        {
            // Implement shape validation logic
            return true;
        }
        
        private bool IsPositionsConnected(int2 pos1, int2 pos2)
        {
            var distance = math.abs(pos1.x - pos2.x) + math.abs(pos1.y - pos2.y);
            
            if (DrawingRule.AllowDiagonalDrawing)
            {
                var maxComponent = math.max(math.abs(pos1.x - pos2.x), math.abs(pos1.y - pos2.y));
                return maxComponent == 1;
            }
            
            return distance == 1;
        }
        
        private int GetTileIndex(int2 position)
        {
            if (position.x < 0 || position.x >= GridSize.x || 
                position.y < 0 || position.y >= GridSize.y)
                return -1;
                
            return position.y * GridSize.x + position.x;
        }

    }
}

