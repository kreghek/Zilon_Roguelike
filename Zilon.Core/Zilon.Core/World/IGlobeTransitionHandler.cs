using System.Threading.Tasks;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public interface IGlobeTransitionHandler
    {
        Task ProcessAsync(IGlobe globe, ISector sector, IActor actor, RoomTransition transition);
    }
}