using System;
using System.Linq;
using System.Threading.Tasks;
using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    /// <summary>
    /// Интерфейс расширителя мира.
    /// Каждый раз, когда кто-нибудь доходит до края мира, мир будет расширяться.
    /// </summary>
    public interface IGlobeExpander
    {
        /// <summary>
        /// Расширение мира в указанной узл.
        /// В результате указанный узел будет материализован и иметь переходы в новые нематериализованные узлы.
        /// </summary>
        /// <param name="sectorNode"> Узел расширения. </param>
        Task ExpandAsync(ISectorNode sectorNode);
    }

    public class BiomeInitializer : IBiomeInitializer, IGlobeExpander
    {
        private readonly IBiomeSchemeRoller _biomeSchemeRoller;
        private readonly ISectorGenerator _sectorGenerator;

        public BiomeInitializer(ISectorGenerator sectorGenerator, IBiomeSchemeRoller biomeSchemeRoller)
        {
            _sectorGenerator = sectorGenerator ?? throw new ArgumentNullException(nameof(sectorGenerator));
            _biomeSchemeRoller = biomeSchemeRoller ?? throw new ArgumentNullException(nameof(biomeSchemeRoller));
        }

        public async Task<IBiome> InitBiomeAsync(ILocationScheme locationScheme)
        {
            var biom = new Biome(locationScheme);

            await CreateStartSectorAsync(biom).ConfigureAwait(false);

            return biom;
        }

        public async Task MaterializeLevelAsync(ISectorNode sectorNode)
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

            // Важно генерировать соседние узлы до начала генерации сектора,
            // чтобы знать переходы из сектора.

            CreateNextSectorNodes(sectorNode, biom);

            var sector = await _sectorGenerator.GenerateAsync(sectorNode).ConfigureAwait(false);

            sectorNode.MaterializeSector(sector);
        }

        public Task ExpandAsync(ISectorNode sectorNode)
        {
            return MaterializeLevelAsync(sectorNode);
        }

        private SectorNode RollAndBindBiome()
        {
            var rolledLocationScheme = _biomeSchemeRoller.Roll();

            var biome = new Biome(rolledLocationScheme);

            var startSectorScheme = biome.LocationScheme.SectorLevels.Single(x => x.IsStart);

            var newBiomeSector = new SectorNode(biome, startSectorScheme);

            return newBiomeSector;
        }

        private async Task CreateStartSectorAsync(IBiome biome)
        {
            var startSectorScheme = biome.LocationScheme.SectorLevels.Single(x => x.IsStart);
            await CreateAndAddSectorByScheme(biome, startSectorScheme).ConfigureAwait(false);
        }

        private async Task CreateAndAddSectorByScheme(IBiome biome, ISectorSubScheme startSectorScheme)
        {
            var sectorNode = new SectorNode(biome, startSectorScheme);

            // Важно генерировать соседние узлы до начала генерации сектора,
            // чтобы знать переходы из сектора.

            biome.AddNode(sectorNode);

            CreateNextSectorNodes(sectorNode, biome);

            var sector = await _sectorGenerator.GenerateAsync(sectorNode).ConfigureAwait(false);

            sectorNode.MaterializeSector(sector);
        }

        private void CreateNextSectorNodes(ISectorNode sectorNode, IBiome biom)
        {
            var nextSectorLevels = biom.LocationScheme.SectorLevels
                .Where(x => sectorNode.SectorScheme.TransSectorSids.Select(trans => trans.SectorLevelSid)
                    .Contains(x.Sid));

            foreach (var nextSectorLevelScheme in nextSectorLevels)
            {
                var nextSectorNode = new SectorNode(biom, nextSectorLevelScheme);

                biom.AddNode(nextSectorNode);

                biom.AddEdge(sectorNode, nextSectorNode);
            }

            // Если в секторе есть переход в другой биом, то
            // Генерируем новый биом, стартовый узел и организуем связь с текущим узлом.
            if (sectorNode.SectorScheme.TransSectorSids.Any(x => x.SectorLevelSid is null))
            {
                var nextSectorNode = RollAndBindBiome();

                // Организуем связь между двумя биомами.

                biom.AddEdge(sectorNode, nextSectorNode);

                var nextBiom = nextSectorNode.Biome;

                nextBiom.AddEdge(sectorNode, nextSectorNode);
            }
        }
    }
}