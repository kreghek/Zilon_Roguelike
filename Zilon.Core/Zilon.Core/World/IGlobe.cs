using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public interface IGlobe
    {
        IEnumerable<ISectorNode> SectorNodes { get; }

        void AddSectorNode(ISectorNode sectorNode);

        Task UpdateAsync();
    }
}