using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public class BiomeInitializer : IBiomeInitializer
    {
        private readonly ISectorGenerator _sectorGenerator;
        private readonly IBiomeSchemeRoller _biomeSchemeRoller;

        public BiomeInitializer(ISectorGenerator sectorGenerator, IBiomeSchemeRoller biomeSchemeRoller)
        {
            _sectorGenerator = sectorGenerator ?? throw new ArgumentNullException(nameof(sectorGenerator));
            _biomeSchemeRoller = biomeSchemeRoller ?? throw new ArgumentNullException(nameof(biomeSchemeRoller));
        }

        public async Task<Biome> InitBiomeAsync(ILocationScheme locationScheme)
        {
            var biom = new Biome(locationScheme);

            await CreateStartSectorAsync(biom).ConfigureAwait(false);

            return biom;
        }

        public async Task MaterializeLevel(SectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            if (sectorNode.State != SectorNodeState.SchemeKnown)
            {
                throw new InvalidOperationException();
            }

            var biom = sectorNode.Biome;

            var sector = await _sectorGenerator.GenerateDungeonAsync(sectorNode.SectorScheme).ConfigureAwait(false);

            sectorNode.MaterializeSector(sector);

            await CreateNextSectorNodesAsync(sectorNode, biom).ConfigureAwait(false);
        }

        private async Task<SectorNode> RollAndBindBiomeAsync()
        {
            var rolledLocationScheme = _biomeSchemeRoller.Roll();

            var biome = new Biome(rolledLocationScheme);

            var startSectorScheme = biome.LocationScheme.SectorLevels.Single(x => x.IsStart);

            var newBiomeSector = new SectorNode(biome, startSectorScheme);

            var sector = await _sectorGenerator.GenerateDungeonAsync(startSectorScheme).ConfigureAwait(false);

            return newBiomeSector;
        }

        private async Task CreateStartSectorAsync(Biome biom)
        {
            var startSectorScheme = biom.LocationScheme.SectorLevels.Single(x => x.IsStart);
            await CreateAndAddSectorByScheme(biom, startSectorScheme).ConfigureAwait(false);
        }

        private async Task CreateAndAddSectorByScheme(Biome biom, ISectorSubScheme startSectorScheme)
        {
            var sector = await _sectorGenerator.GenerateDungeonAsync(startSectorScheme).ConfigureAwait(false);

            var sectorNode = new SectorNode(biom, startSectorScheme);
            sectorNode.MaterializeSector(sector);
            biom.AddNode(sectorNode);

            await CreateNextSectorNodesAsync(sectorNode, biom).ConfigureAwait(false);
        }

        private async Task CreateNextSectorNodesAsync(SectorNode sectorNode, Biome biom)
        {
            var nextSectorLevels = biom.LocationScheme.SectorLevels
                    .Where(x => sectorNode.SectorScheme.TransSectorSids.Select(trans => trans.SectorLevelSid).Contains(x.Sid));

            foreach (var nextSectorLevelScheme in nextSectorLevels)
            {
                var nextSectorNode = new SectorNode(biom, nextSectorLevelScheme);

                biom.AddNode(nextSectorNode);

                biom.AddEdge(sectorNode, nextSectorNode);
            }

            // Если в секторе есть переход в другой биом, то
            // Генерируем новый биом, стартовый узел и организуем связь с текущим узлом.
            if (sectorNode.SectorScheme.TransSectorSids.Any(x=>x.SectorLevelSid is null))
            {
                var nextSectorNode = await RollAndBindBiomeAsync().ConfigureAwait(false);

                // Организуем связь между двумя биомами.

                biom.AddEdge(sectorNode, nextSectorNode);

                var nextBiom = nextSectorNode.Biome;

                nextBiom.AddEdge(sectorNode, nextSectorNode);
            }
        }
    }
}
