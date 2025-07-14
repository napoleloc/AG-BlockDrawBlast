namespace BlockDrawBlast.Gameplay
{
    public readonly record struct DrawingRule
    {
        public DrawingPattern Pattern { get; init; }
        public bool RequiresConnectedPath { get; init; }
        public bool AllowDiagonalDrawing { get; init; }
    }
}

