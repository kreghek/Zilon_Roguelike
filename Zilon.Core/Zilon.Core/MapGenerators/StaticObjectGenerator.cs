﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.StaticObjectFactories;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    public sealed class StaticObjectGenerator : IStaticObjectsGenerator
    {
        private readonly IChestGenerator _chestGenerator;
        private readonly IInteriorObjectRandomSource _interiorObjectRandomSource;
        private readonly IStaticObjectFactoryCollector _staticObjectfactoryCollector;
        private readonly IStaticObjectsGeneratorRandomSource _staticObjectsGeneratorRandomSource;

        [ExcludeFromCodeCoverage]
        public StaticObjectGenerator(IChestGenerator chestGenerator,
            IInteriorObjectRandomSource interiorObjectRandomSource,
            IStaticObjectFactoryCollector staticObjectfactoryCollector,
            IStaticObjectsGeneratorRandomSource staticObjectsGeneratorRandomSource)
        {
            _chestGenerator = chestGenerator ?? throw new ArgumentNullException(nameof(chestGenerator));
            _interiorObjectRandomSource = interiorObjectRandomSource ??
                                          throw new ArgumentNullException(nameof(interiorObjectRandomSource));
            _staticObjectfactoryCollector = staticObjectfactoryCollector ??
                                            throw new ArgumentNullException(nameof(staticObjectfactoryCollector));
            _staticObjectsGeneratorRandomSource = staticObjectsGeneratorRandomSource ??
                                                  throw new ArgumentNullException(
                                                      nameof(staticObjectsGeneratorRandomSource));
        }

        [ExcludeFromCodeCoverage]
        public IMonsterIdentifierGenerator? MonsterIdentifierGenerator { get; set; }

        private async Task CreateInternalAsync(IStaticObjectGenerationContext generationContext)
        {
            var sector = generationContext.Sector;

            await Task.Run(() =>
            {
                var exitNodes = sector.Map.Transitions.Keys.Cast<HexNode>().Select(x => x.OffsetCoords).ToArray();

                // Генерация препятсвий, как статических объектов.

                if (!generationContext.ResourceDepositData.Items.Any())
                {
                    return;
                }

                var checkPass = (sector.Scheme?.Sid?.Contains("globe-node")).GetValueOrDefault();
                foreach (var region in sector.Map.Regions)
                {
                    var regionNodes = region.Nodes.Cast<HexNode>().ToArray();
                    var regionCoords = regionNodes.Select(x => x.OffsetCoords).Except(exitNodes).ToArray();
                    var interiorMetas = _interiorObjectRandomSource.RollInteriorObjects(regionCoords, checkPass);

                    foreach (var interior in interiorMetas)
                    {
                        var node = regionNodes.Single(x => x.OffsetCoords == interior.Coords);
                        var resourceDepositData = generationContext.ResourceDepositData;
                        var staticObject = CreateStaticObject(sector, node, resourceDepositData);

                        sector.StaticObjectManager.Add(staticObject);
                    }
                }

                var sectorSubScheme = generationContext.Scheme;

                if (sectorSubScheme.TotalChestCount > 0)
                {
                    _chestGenerator.CreateChests(sector, sectorSubScheme, sector.Map.Regions);
                }
            });
        }

        private IStaticObject CreateStaticObject(ISector sector, HexNode node, IResourceDepositData resourceDepositData)
        {
            var staticObjectPurpose = RollPurpose(resourceDepositData);

            var factory = _staticObjectfactoryCollector.SelectFactoryByStaticObjectPurpose(staticObjectPurpose);

            var id = default(int);
            if (MonsterIdentifierGenerator != null)
            {
                id = MonsterIdentifierGenerator.GetNewId();
            }

            var staticObject = factory.Create(sector, node, id);

            return staticObject;
        }

        private PropContainerPurpose RollPurpose(IResourceDepositData resourceDepositData)
        {
            return _staticObjectsGeneratorRandomSource.RollPurpose(resourceDepositData);
        }

        public Task CreateAsync(IStaticObjectGenerationContext generationContext)
        {
            if (generationContext is null)
            {
                throw new ArgumentNullException(nameof(generationContext));
            }

            return CreateInternalAsync(generationContext);
        }
    }
}