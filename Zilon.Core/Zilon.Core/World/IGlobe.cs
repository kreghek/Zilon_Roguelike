using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public interface IGlobe
    {
        void AddSectorNode(ISectorNode sectorNode);

        IEnumerable<ISectorNode> SectorNodes { get; }

        Task UpdateAsync();
    }
}