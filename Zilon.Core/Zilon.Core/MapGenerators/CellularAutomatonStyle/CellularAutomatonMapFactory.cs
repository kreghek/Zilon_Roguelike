using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Фабрика карты на основе клеточного автомата.
    /// </summary>
    public sealed class CellularAutomatonMapFactory : IMapFactory
    {
        private const int RETRY_MAX_COUNT = 3;
        private readonly IDice _dice;

        /// <summary>
        /// Конструктор фабрики.
        /// </summary>
        /// <param name="dice"> Кость для рандома. </param>
        public CellularAutomatonMapFactory(IDice dice)
        {
            _dice = dice;
        }

        /// <inheritdoc/>
        public Task<ISectorMap> CreateAsync(ISectorMapFactoryOptions generationOptions)
        {
            if (generationOptions is null)
            {
                throw new ArgumentNullException(nameof(generationOptions));
            }

            var transitions = generationOptions.Transitions;

            var cellularAutomatonOptions = (ISectorCellularAutomataMapFactoryOptionsSubScheme)generationOptions.OptionsSubScheme;
            if (cellularAutomatonOptions == null)
            {
                throw new ArgumentException($"Для {nameof(generationOptions)} не задано {nameof(ISectorSubScheme.MapGeneratorOptions)} равно null.");
            }

            var matrixWidth = cellularAutomatonOptions.MapWidth;
            var matrixHeight = cellularAutomatonOptions.MapHeight;

            var fillProbability = cellularAutomatonOptions.ChanceToStartAlive;

            var cellularAutomatonGenerator = new CellularAutomatonGenerator(_dice);
            var connector = new ClosestRegionConnector();

            var mapRuleManager = new MapRuleManager();
            var rule = new RegionCountRule() { Count = transitions.Count() + 1 };
            mapRuleManager.AddRule(rule);

            var regionPostProcessors = new IRegionPostProcessor[]
            {
                new SplitToTargetCountRegionPostProcessor(mapRuleManager, _dice)
            };

            for (var retryIndex = 0; retryIndex < RETRY_MAX_COUNT; retryIndex++)
            {
                var matrix = new Matrix<bool>(matrixWidth, matrixHeight);

                var regions = cellularAutomatonGenerator.Generate(ref matrix, fillProbability);

                try
                {
                    foreach (var postProcessor in regionPostProcessors)
                    {
                        regions = postProcessor.Process(regions);
                    }

                    connector.Connect(matrix, regions);

                    var map = CreateSectorMap(matrix, regions.ToArray(), transitions);

                    return Task.FromResult(map);
                }
                catch (CellularAutomatonException)
                {
                    // This means that with the current starting data it is not possible to create a suitable map.
                    // Start the next iteration.
                    continue;
                }
            }

            // If the cycle has ended, then no attempt has ended with a successful map building
            throw new InvalidOperationException("Failed to create a map within the maximum number of attempts.");
        }

        private static ISectorMap CreateSectorMap(Matrix<bool> matrix, RegionDraft[] draftRegions, IEnumerable<RoomTransition> transitions)
        {
            // Создание графа карты сектора на основе карты клеточного автомата.
            ISectorMap map = new SectorHexMap();

            FillMapRegions(draftRegions, map);

            MapDraftRegionsToSectorMap(matrix, draftRegions, map);

            // Размещаем переходы и отмечаем стартовую комнату.
            // Общее описание: стараемся размещать переходы в самых маленьких комнатах.
            // Для этого сортируем все комнаты по размеру.
            // Первую занимаем под старт.
            // Последующие - это переходы.

            var regionOrderedBySize = map.Regions.OrderBy(x => x.Nodes.Length).ToArray();

            if (regionOrderedBySize.Any())
            {
                CreateTransitionInSmallestRegion(transitions, map, regionOrderedBySize);
            }

            return map;
        }

        private static void CreateTransitionInSmallestRegion(IEnumerable<RoomTransition> transitions, ISectorMap map, MapRegion[] regionOrderedBySize)
        {
            var startRegion = regionOrderedBySize.First();
            startRegion.IsStart = true;

            var transitionArray = transitions.ToArray();
            // Пропускаем 1, потому что 1 занять стартом.
            var trasitionRegionDrafts = regionOrderedBySize.Skip(1).ToArray();

            Debug.Assert(trasitionRegionDrafts.Length >= transitionArray.Length,
                "Должно быть достаточно регионов для размещения всех переходов.");

            for (var i = 0; i < transitionArray.Length; i++)
            {
                var transitionRegion = trasitionRegionDrafts[i];

                var transition = transitionArray[i];

                var transitionNode = transitionRegion.Nodes.First();

                map.Transitions.Add(transitionNode, transition);

                if (transition.SectorNode == null)
                {
                    transitionRegion.IsOut = true;
                }

                transitionRegion.ExitNodes = (from regionNode in transitionRegion.Nodes
                                              where map.Transitions.Keys.Contains(regionNode)
                                              select regionNode).ToArray();
            }
        }

        private static void FillMapRegions(RegionDraft[] draftRegions, ISectorMap map)
        {
            var regionIdCounter = 1;
            foreach (var draftRegion in draftRegions)
            {
                var regionNodeList = new List<IGraphNode>();

                foreach (var coord in draftRegion.Coords)
                {
                    var node = new HexNode(coord.X, coord.Y);
                    map.AddNode(node);

                    regionNodeList.Add(node);
                }

                var region = new MapRegion(regionIdCounter, regionNodeList.ToArray());

                map.Regions.Add(region);

                regionIdCounter++;
            }
        }

        /// <summary>
        /// Преобразовывет черновые регионы в узлы реальной карты.
        /// </summary>
        private static void MapDraftRegionsToSectorMap(Matrix<bool> matrix, RegionDraft[] draftRegions, ISectorMap map)
        {
            var cellMap = matrix.Items;
            var mapWidth = matrix.Width;
            var mapHeight = matrix.Height;

            var regionNodeCoords = draftRegions.SelectMany(x => x.Coords);
            var hashSet = new HashSet<OffsetCoords>(regionNodeCoords);

            for (var x = 0; x < mapWidth; x++)
            {
                for (var y = 0; y < mapHeight; y++)
                {
                    if (cellMap[x, y])
                    {
                        var offsetCoord = new OffsetCoords(x, y);

                        if (!hashSet.Contains(offsetCoord))
                        {
                            var node = new HexNode(x, y);
                            map.AddNode(node);
                        }
                    }
                }
            }
        }
    }
}