using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public class BiomInitializer : IBiomInitializer
    {
        private readonly ISectorGenerator _sectorGenerator;
        private readonly IBiomSchemeRoller _biomSchemeRoller;

        public BiomInitializer(ISectorGenerator sectorGenerator, IBiomSchemeRoller biomSchemeRoller)
        {
            _sectorGenerator = sectorGenerator ?? throw new ArgumentNullException(nameof(sectorGenerator));
            _biomSchemeRoller = biomSchemeRoller ?? throw new ArgumentNullException(nameof(biomSchemeRoller));
        }

        public async Task<Biom> InitBiomAsync(ILocationScheme locationScheme)
        {
            var biom = new Biom(locationScheme);

            await CreateStartSectorAsync(biom).ConfigureAwait(false);

            return biom;
        }

        public async Task MaterializeLevel(SectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            switch (sectorNode.State)
            {
                case SectorNodeState.SchemeUnknown:
                    await CreateAndAddSectorByScheme(sectorNode.Biom, sectorNode.SectorScheme).ConfigureAwait(false);
                    break;

                case SectorNodeState.SchemeKnown:
                    var newBiomSector = await RollAndBindBiomAsync(sectorNode).ConfigureAwait(false);
                    newBiomSector.Biom.AddNode(newBiomSector);

                    CreateNextSectorNodesAsync(newBiomSector, newBiomSector.Biom);
                    break;
            }
        }

        private async Task<SectorNode> RollAndBindBiomAsync(SectorNode sectorNode)
        {
            var rolledLocationScheme = _biomSchemeRoller.Roll();

            var biom = new Biom(rolledLocationScheme);

            var startSectorScheme = biom.LocationScheme.SectorLevels.Single(x => x.IsStart);

            sectorNode.BindSchemeInfo(biom, startSectorScheme);

            var sector = await _sectorGenerator.GenerateDungeonAsync(startSectorScheme).ConfigureAwait(false);

            sectorNode.MaterializeSector(sector);

            return sectorNode;
        }

        private async Task CreateStartSectorAsync(Biom biom)
        {
            var startSectorScheme = biom.LocationScheme.SectorLevels.Single(x => x.IsStart);
            await CreateAndAddSectorByScheme(biom, startSectorScheme).ConfigureAwait(false);
        }

        private async Task CreateAndAddSectorByScheme(Biom biom, ISectorSubScheme startSectorScheme)
        {
            var sector = await _sectorGenerator.GenerateDungeonAsync(startSectorScheme).ConfigureAwait(false);

            var sectorNode = new SectorNode(biom, startSectorScheme);
            sectorNode.MaterializeSector(sector);
            biom.AddNode(sectorNode);

            await CreateNextSectorNodesAsync(sectorNode, biom).ConfigureAwait(false);
        }

        private static async Task CreateNextSectorNodesAsync(SectorNode sectorNode, Biom biom)
        {
            if (sectorNode.SectorScheme.TransSectorSids is null)
            {
                var next = await RollAndBindBiomAsync(sectorNode);

                return;
            }

            var nextSectorLevels = biom.LocationScheme.SectorLevels.Where(x => sectorNode.SectorScheme.TransSectorSids.Contains(x.Sid));

            foreach (var nextSectorLevelScheme in nextSectorLevels)
            {
                var nextSectorNode = new SectorNode(biom, nextSectorLevelScheme);

                biom.AddNode(nextSectorNode);

                biom.AddEdge(sectorNode, nextSectorNode);
            }
        }
    }
}
