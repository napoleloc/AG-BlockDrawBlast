using System.Threading;
using Cysharp.Threading.Tasks;
using EncosyTower.StringIds;

namespace BlockDrawBlast.Gameplay
{
    public interface IMonoBlockVisual { }

    public interface IMonoBlockVisualCreateAsync : IMonoBlockVisual
    {
        UniTask OnCreateAsync(BlockData blockData, CancellationToken token);
    }

    public interface IMonoBlockVisualHasEffect : IMonoBlockVisual
    {
        IMonoBlockVisualEffect MonoBlockVisualEffect { get; }
    }

    public interface IMonoBlockVisualEffect { }
    
    
    public interface IMonoBlockVisualDestructEffectAsync : IMonoBlockVisualEffect
    {
        UniTask OnPlayDestructAsync(CancellationToken token);
    }
    
    public interface IMonoBlockVisualSpawnEffectAsync : IMonoBlockVisualEffect
    {
        UniTask OnPlaySpawnAsync(CancellationToken token);
    }
}

