using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public interface IGameManager
    {
        Task RequestNextActorTaskAsync();

        IActorTaskSource[] ActorTaskSources { get; set; }
    }
}