using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    public interface ISectorMapFactoryOptions
    {
        ISectorMapFactoryOptionsSubScheme OptionsSubScheme { get; }

        IEnumerable<RoomTransition> Transitions { get; }
    }
}