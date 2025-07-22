using EncosyTower.PubSub;

namespace BlockDrawBlast.Gameplay
{
    public readonly record struct GameplayScope();
    
    public readonly record struct GameStartedMessage : IMessage;
    public readonly record struct GamePausedMessage : IMessage;
    public readonly record struct GameResumedMessage : IMessage;
    
    public readonly record struct LevelFailedMessage : IMessage;
    public readonly record struct LevelCompletedMessage : IMessage;
    
    public readonly record struct ScoreChangedMessage(int newScore) : IMessage;
    public readonly record struct ComboExtendedMessage(int comboCount, int bonusScore) : IMessage;
}