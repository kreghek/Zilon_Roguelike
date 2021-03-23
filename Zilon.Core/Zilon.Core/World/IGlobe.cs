using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public interface IGlobe
    {
        IEnumerable<ISectorNode> SectorNodes { get; }

        void AddSectorNode(ISectorNode sectorNode);

        Task UpdateAsync(CancellationToken cancelToken);

        IGlobeIterationMarker CurrentIteration { get; }
    }

    public interface IGlobeIterationMarker
    {
        public string Name { get; }
    }

    public sealed class GlobeIterationMarker : IGlobeIterationMarker
    {
        public string Name
        {
            get
            {
                return GetHashCode().ToString();
            }
        }
    }
}