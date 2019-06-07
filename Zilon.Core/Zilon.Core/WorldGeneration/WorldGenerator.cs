using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration.AgentCards;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Экземпляр генератора мира с историей.
    /// </summary>
    /// <seealso cref="IWorldGenerator" />
    public class WorldGenerator : IWorldGenerator
    {
        private const int Size = 10;
        private const int StartRealmCount = 4;
        private const int HistoryIterationCount = 40;
        private const int StartAgentCount = 40;
        private const int LocationBaseSize = 20;
        private const string CITY_SCHEME_SID = "city";
        private const string WILD_SCHEME_SID = "forest";
        private const int BORDER_SIZE = 1;

        private readonly IDice _dice;
        private readonly ISchemeService _schemeService;

        /// <summary>
        /// Создаёт экземпляр <see cref="WorldGenerator"/>.
        /// </summary>
        /// <param name="dice"> Игровая кость, которая будет использована для рандомицзации событий мира.
        /// В ближайшее время будет заменена специализированным источником рандома.
        /// </param>
        /// <param name="schemeService"> Сервис для доступа к схемам. Используется для генерации карты локации в провинции. </param>
        public WorldGenerator(IDice dice, ISchemeService schemeService)
        {
            _dice = dice;
            _schemeService = schemeService;
        }

        /// <summary>
        /// Создание игрового мира с историей и граф провинций.
        /// </summary>
        /// <returns>
        /// Возвращает объект игрового мира.
        /// </returns>
        public Task<Globe> GenerateGlobeAsync()
        {
            var globe = new Globe
            {
                Terrain = new TerrainCell[Size][]
            };

            var realmTask = CreateRealms(globe);
            var terrainTask = CreateTerrain(globe);

            Task.WaitAll(realmTask, terrainTask);

            CreateStartLocalities(globe);
            CreateStartAgents(globe);

            var agentsClock = new Stopwatch();
            agentsClock.Start();

            var cardQueue = CreateAgentCardQueue();

            // обработка итераций
            ProcessIterations(globe, cardQueue);


            globe.StartProvince = GetStartProvinceCoords(globe);
            globe.HomeProvince = GetHomeProvinceCoords(globe, globe.StartProvince);

            agentsClock.Stop();
            Console.WriteLine(agentsClock.ElapsedMilliseconds / 1f + "s");

            return Task.FromResult(globe);
        }

        private TerrainCell GetStartProvinceCoords(Globe globe)
        {
            // Выбираем основной город, являющийся стартовым.
            // По городу определяем провинцию.

            var availableStartLocalities = globe.Localities;

            Locality startLocality;
            if (availableStartLocalities.Any())
            {
                var startLocalityIndex = _dice.Roll(0, availableStartLocalities.Count() - 1);
                startLocality = availableStartLocalities[startLocalityIndex];
            }
            else
            {
                startLocality = globe.Localities.FirstOrDefault();
            }

            if (startLocality == null)
            {
                //TODO Создать отдельный класс исключений GlobeGenerationException.
                throw new InvalidOperationException("Не удалось выбрать стартовую локацию.");
            }

            return startLocality.Cell;
        }

        //TODO Постараться объединить GetStartProvinceCoords и GetHomeProvinceCoords
        private TerrainCell GetHomeProvinceCoords(Globe globe, TerrainCell startProvince)
        {
            // Выбираем основной город, являющийся стартовым.
            // По городу определяем провинцию.

            var currentCubeCoords = HexHelper.ConvertToCube(startProvince.Coords);
            var availableLocalities = globe.Localities
                .Where(x => x.Cell != startProvince)
                .Where(x => HexHelper.ConvertToCube(x.Cell.Coords).DistanceTo(currentCubeCoords) > 2)
                .Where(x => HexHelper.ConvertToCube(x.Cell.Coords).DistanceTo(currentCubeCoords) <= 5)
                .ToArray();

            Locality selectedLocality;
            if (availableLocalities.Any())
            {
                var localityIndex = _dice.Roll(0, availableLocalities.Count() - 1);
                selectedLocality = availableLocalities[localityIndex];
            }
            else
            {
                selectedLocality = globe.Localities.LastOrDefault();
            }

            if (selectedLocality == null)
            {
                //TODO Создать отдельный класс исключений GlobeGenerationException.
                throw new InvalidOperationException("Не удалось выбрать локацию для дома.");
            }

            return selectedLocality.Cell;
        }

        /// <summary>
        /// Создание
        /// </summary>
        /// <param name="globe">Объект игрового мира, для которого создаётся локация.</param>
        /// <param name="cell">Провинция игрового мира из указанного выше <see cref="Globe" />,
        /// для которого создаётся локация.</param>
        /// <returns>
        /// Возвращает граф локация для провинции.
        /// </returns>
        public Task<GlobeRegion> GenerateRegionAsync(Globe globe, TerrainCell cell)
        {
            var locationSchemeSids = new[] {
                "rat-hole",
                "rat-kingdom",
                "demon-dungeon",
                "demon-lair",
                "crypt",
                "elder-place",
                "genomass-cave"
            };
            var region = new GlobeRegion(LocationBaseSize);

            var isStartCell = globe.StartProvince == cell;
            OffsetCoords startOffsetCoords = null;
            if (isStartCell)
            {
                startOffsetCoords = GetStartCoords(LocationBaseSize);
            }

            var isHomeCell = globe.HomeProvince == cell;
            OffsetCoords homeOffsetCoords = null;
            if (isHomeCell)
            {
                homeOffsetCoords = GetHomeCoords(LocationBaseSize);
            }

            
            for (var x = 0; x < LocationBaseSize; x++)
            {
                for (var y = 0; y < LocationBaseSize; y++)
                {
                    // Эти строки вставляют непроходимые места в карте.
                    // Сейчас это не безопасно, потому что могут генерироваться изолированные
                    // куски карты провинции.
                    // Дополнительно этот фрагмент может создать края провинции,
                    // которые не будут совпадать при переходе.
                    // Второе можно быстро решить, запретив делать непроходимые узлы на границе.
                    //var hasNodeRoll = _dice.Roll(6);
                    //if (hasNodeRoll <= 2)
                    //{
                    //    continue;
                    //}

                    var isBorder = x == 0 || x == LocationBaseSize - BORDER_SIZE || y == 0 || y == LocationBaseSize - BORDER_SIZE;

                    if (isBorder)
                    {
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
                        var node = new GlobeRegionNode(x, y, locationScheme)
                        {
                            IsBorder = isBorder
                        };
                        region.AddNode(node);
                    }
                    else
                    {
                        if (isStartCell && x == startOffsetCoords.X && y == startOffsetCoords.Y)
                        {
                            var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
                            var node = new GlobeRegionNode(x, y, locationScheme)
                            {
                                IsStart = true
                            };
                            region.AddNode(node);
                        }
                        else if (isHomeCell && x == homeOffsetCoords.X && y == homeOffsetCoords.Y)
                        {
                            var locationScheme = _schemeService.GetScheme<ILocationScheme>(CITY_SCHEME_SID);
                            var node = new GlobeRegionNode(x, y, locationScheme)
                            {
                                IsTown = true,
                                IsHome = true
                            };
                            region.AddNode(node);
                        }
                        else
                        {
                            var hasDundeonRoll = _dice.Roll(100);
                            if (hasDundeonRoll > 90)
                            {
                                var locationSidIndex = _dice.Roll(0, locationSchemeSids.Length - 1);
                                var locationSid = locationSchemeSids[locationSidIndex];
                                var locationScheme = _schemeService.GetScheme<ILocationScheme>(locationSid);
                                var node = new GlobeRegionNode(x, y, locationScheme);
                                region.AddNode(node);
                            }
                            else
                            {
                                var hasCityRoll = _dice.Roll(100);

                                if (hasCityRoll > 90)
                                {
                                    var locationScheme = _schemeService.GetScheme<ILocationScheme>(CITY_SCHEME_SID);
                                    var node = new GlobeRegionNode(x, y, locationScheme)
                                    {
                                        IsTown = true
                                    };
                                    region.AddNode(node);
                                }
                                else
                                {
                                    var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
                                    var node = new GlobeRegionNode(x, y, locationScheme);
                                    region.AddNode(node);
                                }
                            }
                        }
                    }
                }
            }

            return Task.FromResult(region);
        }

        private OffsetCoords GetHomeCoords(int locationBaseSize)
        {
            var homeX = GetHomeCoordsComponent(locationBaseSize);
            var homeY = GetHomeCoordsComponent(locationBaseSize);
            return new OffsetCoords(homeX, homeY);
        }

        private OffsetCoords GetStartCoords(int locationBaseSize)
        {
            var startX = locationBaseSize / 2;
            var startY = locationBaseSize / 2;
            return new OffsetCoords(startX, startY);
        }

        private int GetHomeCoordsComponent(int locationBaseSize)
        {
            // Стараемся выбрать победный узел с краю, но не на границе.
            var leftBorder = locationBaseSize / 4;
            var rigthBorder = locationBaseSize * 3 / 4;

            var safityCounter = 100;
            while (safityCounter > 0)
            {
                var component = _dice.Roll(BORDER_SIZE, locationBaseSize - 1 - BORDER_SIZE);

                // Определение, что выбранная координата внутри диапазона.
                var isInner = leftBorder <= component && component <= rigthBorder;
                if (!isInner)
                {
                    return component;
                }

                safityCounter--;
            }

            // Если попали сюда, то возвращаем фиксированную центральную точку.
            return locationBaseSize / 2;
        }

        private void ProcessIterations(Globe globe, Queue<IAgentCard> cardQueue)
        {
            for (var year = 0; year < HistoryIterationCount; year++)
            {
                foreach (var agent in globe.Agents.ToArray())
                {
                    var card = cardQueue.Dequeue();

                    if (card.CanUse(agent, globe))
                    {
                        card.Use(agent, globe, _dice);
                    }

                    cardQueue.Enqueue(card);
                }
            }
        }

        private static Queue<IAgentCard> CreateAgentCardQueue()
        {
            return new Queue<IAgentCard>(new IAgentCard[] {
                new ChangeLocality(),
                new CreateLocality(),
                new IncreasePopulation(),
                new AgentOpposition(),
                new AgentSupport(),
                new Disciple(),
                new TakeLocation()
            });
        }

        private void CreateStartAgents(Globe globe)
        {
            for (var i = 0; i < StartAgentCount; i++)
            {
                var rolledLocalityIndex = _dice.Roll(0, globe.Localities.Count - 1);
                var locality = globe.Localities[rolledLocalityIndex];

                var agent = new Agent
                {
                    Name = $"agent {i}",
                    Localtion = locality.Cell,
                    Realm = locality.Owner
                };

                globe.Agents.Add(agent);

                Helper.AddAgentToCell(globe.AgentCells, locality.Cell, agent);

                var rolledBranchIndex = _dice.Roll(0, 7);
                agent.Skills = new Dictionary<BranchType, int>
                {
                    { (BranchType)rolledBranchIndex, 1 }
                };
            }
        }

        private void CreateStartLocalities(Globe globe)
        {
            for (var i = 0; i < StartRealmCount; i++)
            {
                var randomX = _dice.Roll(0, Size - 1);
                var randomY = _dice.Roll(0, Size - 1);

                var locality = new Locality()
                {
                    Name = $"L{i}",
                    Cell = globe.Terrain[randomX][randomY],
                    Owner = globe.Realms[i],
                    Population = 3
                };

                var rolledBranchIndex = _dice.Roll(0, 7);
                locality.Branches = new Dictionary<BranchType, int>
                        {
                            { (BranchType)rolledBranchIndex, 1 }
                        };

                globe.Localities.Add(locality);

                globe.LocalitiesCells[locality.Cell] = locality;

                globe.ScanResult.Free.Remove(locality.Cell);
            }
        }

        private Task CreateTerrain(Globe globe)
        {
            for (var i = 0; i < Size; i++)
            {
                globe.Terrain[i] = new TerrainCell[Size];

                for (var j = 0; j < Size; j++)
                {
                    globe.Terrain[i][j] = new TerrainCell
                    {
                        Coords = new OffsetCoords(i, j)
                    };

                    globe.ScanResult.Free.Add(globe.Terrain[i][j]);
                }
            }

            return Task.CompletedTask;
        }

        private Task CreateRealms(Globe globe)
        {
            var realmColors = new[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };
            for (var i = 0; i < StartRealmCount; i++)
            {
                var realm = new Realm
                {
                    Name = $"realm {i}",
                    Color = realmColors[i]
                };

                globe.Realms.Add(realm);
            }

            return Task.CompletedTask;
        }
    }
}
