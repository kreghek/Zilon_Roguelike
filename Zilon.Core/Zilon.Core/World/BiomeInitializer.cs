﻿using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public class BiomeInitializer : IBiomeInitializer, IGlobeExpander
    {
        private const int NEXT_BIOME_COUNT = 3;

        private readonly IBiomeSchemeRoller _biomeSchemeRoller;
        private readonly ISectorGenerator _sectorGenerator;

        public BiomeInitializer(ISectorGenerator sectorGenerator, IBiomeSchemeRoller biomeSchemeRoller)
        {
            _sectorGenerator = sectorGenerator ?? throw new ArgumentNullException(nameof(sectorGenerator));
            _biomeSchemeRoller = biomeSchemeRoller ?? throw new ArgumentNullException(nameof(biomeSchemeRoller));
        }

        private async Task CreateAndAddSectorBySchemeAsync(IBiome biome, ISectorSubScheme startSectorScheme)
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
                .Where(x => IsHasTransitionToNextLevel(sectorNode, x));

            foreach (var nextSectorLevelScheme in nextSectorLevels)
            {
                var nextSectorNode = new SectorNode(biom, nextSectorLevelScheme);

                biom.AddNode(nextSectorNode);

                biom.AddEdge(sectorNode, nextSectorNode);
            }

            // Если в секторе есть переход в другой биом, то
            // Генерируем новый биом, стартовый узел и организуем связь с текущим узлом.
            var isTransitionToNextBiom =
                sectorNode.SectorScheme.TransSectorSids.Any(sid => sid?.SectorLevelSid is null);
            if (isTransitionToNextBiom)
            {
                var nextBiomeCount = NEXT_BIOME_COUNT;

                for (var nextBiomeIndex = 0; nextBiomeIndex < nextBiomeCount; nextBiomeIndex++)
                {
                    var nextSectorNode = RollAndBindBiome();

                    // Организуем связь между двумя биомами.

                    biom.AddEdge(sectorNode, nextSectorNode);

                    var nextBiom = nextSectorNode.Biome;

                    nextBiom.AddEdge(sectorNode, nextSectorNode);
                }
            }
        }

        private async Task CreateStartSectorAsync(IBiome biome)
        {
            var startSectorScheme = biome.LocationScheme.SectorLevels.Single(x => x.IsStart);
            await CreateAndAddSectorBySchemeAsync(biome, startSectorScheme).ConfigureAwait(false);
        }

        private static bool IsHasTransitionToNextLevel(ISectorNode sectorNode, ISectorSubScheme x)
        {
            return sectorNode.SectorScheme.TransSectorSids
                .Where(x => x != null).Select(x => x!)
                .Select(trans => trans.SectorLevelSid)
                .Contains(x.Sid);
        }

        private SectorNode RollAndBindBiome()
        {
            var rolledLocationScheme = _biomeSchemeRoller.Roll();

            var biome = new Biome(rolledLocationScheme);

            var startSectorScheme = biome.LocationScheme.SectorLevels.Single(x => x.IsStart);

            var newBiomeSector = new SectorNode(biome, startSectorScheme);

            return newBiomeSector;
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
    }
}