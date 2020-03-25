using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    public class SectorMapFactoryOptions : ISectorMapFactoryOptions
    {
        public ISectorMapFactoryOptionsSubScheme OptionsSubScheme { get; set; }
        public IEnumerable<RoomTransition> Transitions { get; set; }
    }
}
