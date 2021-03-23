using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public interface IGlobe
    {
        IGlobeIterationMarker CurrentIteration { get; }
        IEnumerable<ISectorNode> SectorNodes { get; }

        void AddSectorNode(ISectorNode sectorNode);

        Task UpdateAsync(CancellationToken cancelToken);
    }

    public interface IGlobeIterationMarker
    {
        public string Name { get; }
    }

    public sealed class GlobeIterationMarker : IGlobeIterationMarker
    {
        public string Name => GetHashCode().ToString();
    }
}