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
            var isHomeCell = globe.HomeProvince == cell;

            // Сейчас допускаем, что паттерны квадратные, меньше размера провинции.
            // Пока не вращаем и не искажаем.
            // Там, где может быть объект, гарантированно создаём один город и два подземелья.
            var regionDraft = new GlobeRegionDraftValue[LocationBaseSize, LocationBaseSize];
            // Вставляем паттерны в указанные области
            var defaultPattern = GlobeRegionPatterns.Default;
            var startPattern = GlobeRegionPatterns.Start;
            var homePattern = GlobeRegionPatterns.Home;
            var patternSize = defaultPattern.Values.GetUpperBound(0);

            ApplyRegionPattern(ref regionDraft, defaultPattern, 1, 1);
            ApplyRegionPattern(ref regionDraft, defaultPattern, LocationBaseSize - patternSize - 1, 1);
            ApplyRegionPattern(ref regionDraft, defaultPattern, 1, LocationBaseSize - patternSize - 1);
            ApplyRegionPattern(ref regionDraft, defaultPattern, LocationBaseSize - patternSize - 1, LocationBaseSize - patternSize - 1);
            if (isStartCell)
            {
                ApplyRegionPattern(ref regionDraft, startPattern, (LocationBaseSize - patternSize) / 2, (LocationBaseSize - patternSize) / 2);
            }
            else if (isHomeCell)
            {
                ApplyRegionPattern(ref regionDraft, homePattern, (LocationBaseSize - patternSize) / 2, (LocationBaseSize - patternSize) / 2);
            }
            else
            {
                ApplyRegionPattern(ref regionDraft, defaultPattern, (LocationBaseSize - patternSize) / 2, (LocationBaseSize - patternSize) / 2);
            }


            for (var x = regionDraft.GetLowerBound(0); x < regionDraft.GetUpperBound(0); x++)
            {
                for (var y = regionDraft.GetLowerBound(1); y < regionDraft.GetUpperBound(1); y++)
                {
                    // Определяем, является ли узел граничным.
                    // На граничных узлах ничего не создаём.
                    // Потому что это может вызвать трудности при переходах между провинциями.
                    // Например, игрок при переходе сразу может попасть в данж или город.
                    // Не отлажен механиз перехода, если часть узлов соседней провинции отсутствует.
                    var isBorder = x == 0 || x == LocationBaseSize - BORDER_SIZE || y == 0 || y == LocationBaseSize - BORDER_SIZE;
                    if (isBorder)
                    {
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
                        var borderNode = new GlobeRegionNode(x, y, locationScheme)
                        {
                            IsBorder = isBorder
                        };
                        region.AddNode(borderNode);
                        continue;
                    }

                    var currentPatternValue = regionDraft[x, y];
                    GlobeRegionNode node = null;
                    if (currentPatternValue == null)
                    {
                        // Ничего не делаем.
                        // Это генерирует остутствие узла, что сейчас значит непроходимый узел.
                    }
                    else if (currentPatternValue.Value.HasFlag(GlobeRegionDraftValueType.Wild))
                    {
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
                        node = new GlobeRegionNode(x, y, locationScheme);
                        
                    }
                    else if (currentPatternValue.IsStart)
                    {
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
                        node = new GlobeRegionNode(x, y, locationScheme)
                        {
                            IsStart = true
                        };
                    }
                    else if (currentPatternValue.IsHome)
                    {
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>(CITY_SCHEME_SID);
                        node = new GlobeRegionNode(x, y, locationScheme)
                        {
                            IsTown = true,
                            IsHome = true
                        };
                    }
                    else if (currentPatternValue.Value.HasFlag(GlobeRegionDraftValueType.Town))
                    {
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>(CITY_SCHEME_SID);
                        node = new GlobeRegionNode(x, y, locationScheme)
                        {
                            IsTown = true
                        };
                    }
                    else if (currentPatternValue.Value.HasFlag(GlobeRegionDraftValueType.Dungeon))
                    {
                        var locationSidIndex = _dice.Roll(0, locationSchemeSids.Length - 1);
                        var locationSid = locationSchemeSids[locationSidIndex];
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>(locationSid);
                        node = new GlobeRegionNode(x, y, locationScheme);
                    }
                    else
                    {
                        Debug.Assert(true, "При генерации провинции должны все исходы быть предусмотрены.");
                    }

                    if (node != null)
                    {
                        region.AddNode(node);
                    }
                }
            }

            return Task.FromResult(region);
        }

        /// <summary>
        /// Применяет шаблон участка провинции на черновик провинции в указанных координатах.
        /// </summary>
        /// <param name="regionDraft"> Черновик провинции.
        /// Отмечен ref, чтобы было видно, что метод именяет этот объект. </param>
        /// <param name="pattern"> Шаблон, применяемый на черновик. </param>
        /// <param name="insertX"> Х-координата применения шаблона. </param>
        /// <param name="insertY"> Y-координата применения шаблона. </param>
        private void ApplyRegionPattern(ref GlobeRegionDraftValue[,] regionDraft,
                                         GlobeRegionPattern pattern,
                                         int insertX,
                                         int insertY)
        {
            // Пока костыльное решение из расчёта, что во всех паттернах будет 3 объекта интереса.

            var townIndex = _dice.Roll(0, 2);
            var interestObjectCounter = 0;
            for (var x = pattern.Values.GetLowerBound(0); x < pattern.Values.GetUpperBound(0); x++)
            {
                for (var y = pattern.Values.GetLowerBound(1); y < pattern.Values.GetUpperBound(1); y++)
                {
                    GlobeRegionDraftValue draftValue = null;

                    // TODO Для диких секторов нужно ввести отдельное значение шаблона.
                    // Потому что в шаблонах планируются ещё пустые (непроходимые) узлы. Но и для них будет специальное значение.
                    // А пустота (null) будет означать, что ничего делать не нужно (transparent).
                    // Потому что будут ещё шаблоны, накладываемые поверх всех в случайные места, чтобы генерировать дополнительные 
                    // мусорные объекты.
                    var patternValue = pattern.Values[x, y];
                    if (patternValue == null)
                    {
                        draftValue = new GlobeRegionDraftValue(GlobeRegionDraftValueType.Wild);
                    }
                    else if (patternValue.IsStart)
                    {
                        draftValue = new GlobeRegionDraftValue(GlobeRegionDraftValueType.Dungeon)
                        {
                            IsStart = true
                        };
                    }
                    else if (patternValue.IsHome)
                    {
                        draftValue = new GlobeRegionDraftValue(GlobeRegionDraftValueType.Town)
                        {
                            IsHome = true
                        };
                    }
                    else if (patternValue.HasObject)
                    {
                        if (interestObjectCounter == townIndex)
                        {
                            draftValue = new GlobeRegionDraftValue(GlobeRegionDraftValueType.Town);
                        }
                        else
                        {
                            draftValue = new GlobeRegionDraftValue(GlobeRegionDraftValueType.Dungeon);
                        }
                        interestObjectCounter++;
                    }

                    regionDraft[x + insertX, y + insertY] = draftValue;
                }
            }
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
