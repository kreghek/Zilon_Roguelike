using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public interface IGlobeTransitionHandler
    {
        Task InitActorTransitionAsync(IGlobe globe, ISector sector, IActor actor, SectorTransition transition);

        void UpdateTransitions();
    }
}