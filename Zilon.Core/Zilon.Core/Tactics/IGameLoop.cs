using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public interface IGameLoop
    {
        void Update();

        IActorTaskSource[] ActorTaskSources { get; set; }
    }
}