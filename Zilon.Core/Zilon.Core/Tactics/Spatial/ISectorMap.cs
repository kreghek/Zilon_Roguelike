using System.Collections.Generic;

using Zilon.Core.MapGenerators.RoomStyle;

namespace Zilon.Core.Tactics.Spatial
{
    public interface ISectorMap: IMap
    {
        Dictionary<IMapNode, RoomTransition> Transitions { get; }
    }
}
