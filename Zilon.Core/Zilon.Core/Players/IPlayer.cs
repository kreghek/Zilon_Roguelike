using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.Core.Players
{
    public interface IPlayer
    {
        ISectorNode SectorNode { get; }
        IPerson MainPerson { get; set; }

        void BindSectorNode(ISectorNode sectorNode);
        void Reset();
    }
}
