using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public class BiomService
    {
        private readonly ISectorGenerator _sectorGenerator;

        public async Task<Biom> InitBiom(ILocationScheme locationScheme)
        {
            var biom = new Biom(locationScheme);

            await CreateStartSectorAsync(biom).ConfigureAwait(false);

            return biom;
        }

        private async Task CreateStartSectorAsync(Biom biom)
        {
            var startSectorScheme = biom.LocationScheme.SectorLevels.Single(x => x.IsStart);
            var sector = await _sectorGenerator.GenerateDungeonAsync(startSectorScheme).ConfigureAwait(false);

            var sectorNode = new SectorNode(startSectorScheme);
            sectorNode.MaterializeSector(sector);
            biom.AddNode(sectorNode);

            CreateNextSectorNodes(sectorNode, biom);
        }

        private static void CreateNextSectorNodes(SectorNode sectorNode, Biom biom)
        {
            var nextSectorLevels = biom.LocationScheme.SectorLevels.Where(x => sectorNode.SectorScheme.TransSectorSids.Contains(x.Sid));

            foreach (var nextSectorLevelScheme in nextSectorLevels)
            {
                var nextSectorNode = new SectorNode(nextSectorLevelScheme);
                
                biom.AddNode(sectorNode);

                biom.AddEdge(sectorNode, nextSectorNode);
            }
        }
    }
}
