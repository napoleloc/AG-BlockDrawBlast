using System.Threading;
using Cysharp.Threading.Tasks;

namespace BlockDrawBlast.Gameplay
{
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

