using System.Diagnostics;
using System.Threading.Tasks;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public sealed class ProvinceInitiator
    {
        private const int ProvinceBaseSize = 20;
        private const string CITY_SCHEME_SID = "city";
        private const string WILD_SCHEME_SID = "forest";

        private readonly IDice _dice;
        private readonly ISchemeService _schemeService;

        public ProvinceInitiator(IDice dice, ISchemeService schemeService)
        {
            _dice = dice;
            _schemeService = schemeService;
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
        public Task<Province> GenerateProvinceAsync()
        {
            return Task.Run(() =>
            {
                var province = new Province(ProvinceBaseSize);

                // Сейчас допускаем, что паттерны квадратные, меньше размера провинции.
                // Пока не вращаем и не искажаем.
                // Там, где может быть объект, гарантированно создаём один город и два подземелья.
                var regionDraft = new GlobeRegionDraftValue[ProvinceBaseSize, ProvinceBaseSize];
                var startPattern = GlobeRegionPatterns.Start;
                var homePattern = GlobeRegionPatterns.Home;
                // Расчитываем размер паттернов.
                // Исходим из того, что пока все паттерны квадратные и одинаковые по размеру.
                // Поэтому размер произвольного паттерна будет справедлив для всех остальных.
                // Паттерн старта выбран произвольно.
                var patternSize = startPattern.Values.GetUpperBound(0) - startPattern.Values.GetLowerBound(0) + 1;

                // Вставляем паттерны в указанные области
                RotateAndApplyRegionPattern(ref regionDraft, GetDefaultPattrn(), 1, 1);
                RotateAndApplyRegionPattern(ref regionDraft, GetDefaultPattrn(), ProvinceBaseSize - patternSize - 1, 1);
                RotateAndApplyRegionPattern(ref regionDraft, GetDefaultPattrn(), 1, ProvinceBaseSize - patternSize - 1);
                RotateAndApplyRegionPattern(ref regionDraft, GetDefaultPattrn(), ProvinceBaseSize - patternSize - 1, ProvinceBaseSize - patternSize - 1);
                RotateAndApplyRegionPattern(ref regionDraft, GetDefaultPattrn(), (ProvinceBaseSize - patternSize) / 2, (ProvinceBaseSize - patternSize) / 2);

                // На основе черновика генерируем злы провинции
                AddProvinceNodesFromDraft(province, regionDraft);

                return province;
            });
        }

        private void AddProvinceNodesFromDraft(Province province, GlobeRegionDraftValue[,] regionDraft)
        {
            for (var x = 0; x <= ProvinceBaseSize; x++)
            {
                for (var y = 0; y <= ProvinceBaseSize; y++)
                {
                    // Определяем, является ли узел граничным. На граничных узлах ничего не создаём.
                    // Потому что это может вызвать трудности при переходах между провинциями.
                    // Например, игрок при переходе сразу может попасть в данж или город.
                    // Не отлажен механиз перехода, если часть узлов соседней провинции отсутствует.
                    var isBorder = GetIsBorder(x, y);
                    if (isBorder)
                    {
                        AddBorderNode(province, x, y);
                    }
                    else
                    {
                        var currentPatternValue = regionDraft[x, y];

                        AddNode(province, currentPatternValue, x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Применяет шаблон участка провинции на черновик провинции в указанных координатах.
        /// </summary>
        /// <param name="regionDraft"> Черновик провинции.
        /// Отмечен ref, чтобы было видно, что метод именяет этот объект. </param>
        /// <param name="pattern"> Шаблон, применяемый на черновик. </param>
        /// <param name="insertX"> Х-координата применения шаблона. </param>
        /// <param name="insertY"> Y-координата применения шаблона. </param>
        private void RotateAndApplyRegionPattern(ref GlobeRegionDraftValue[,] regionDraft,
                                         GlobeRegionPattern pattern,
                                         int insertX,
                                         int insertY)
        {
            // Пока костыльное решение из расчёта, что во всех паттернах будет 3 объекта интереса.
            var townCount = CountTownPlaces(pattern.Values);
            var townIndex = _dice.Roll(0, townCount - 1);

            var rotateValueIndex = _dice.Roll(0, (int)MatrixRotation.ConterClockwise90);
            GlobeRegionPatternValue[,] rotatedPatternValues = RotatePattern(pattern, rotateValueIndex);
            ApplyRegionPattern(regionDraft, rotatedPatternValues, insertX, insertY, townIndex);
        }

        private static GlobeRegionPatternValue[,] RotatePattern(GlobeRegionPattern pattern, int rotateValueIndex)
        {
            return MatrixHelper.Rotate(pattern.Values, (MatrixRotation)rotateValueIndex);
        }

        private static int CountTownPlaces(GlobeRegionPatternValue[,] patternValues)
        {
            var counter = 0;
            foreach (var value in patternValues)
            {
                if (value == null)
                {
                    continue;
                }

                if (value.HasObject)
                {
                    counter++;
                }
            }

            return counter;
        }

        private static void ApplyRegionPattern(GlobeRegionDraftValue[,] regionDraft,
                                                    GlobeRegionPatternValue[,] patternValues,
                                                    int insertX,
                                                    int insertY,
                                                    int townIndex)
        {
            var interestObjectCounter = 0;

            for (var x = patternValues.GetLowerBound(0); x <= patternValues.GetUpperBound(0); x++)
            {
                for (var y = patternValues.GetLowerBound(1); y <= patternValues.GetUpperBound(1); y++)
                {
                    GlobeRegionDraftValue draftValue = null;

                    // TODO Для диких секторов нужно ввести отдельное значение шаблона.
                    // Потому что в шаблонах планируются ещё пустые (непроходимые) узлы. Но и для них будет специальное значение.
                    // А пустота (null) будет означать, что ничего делать не нужно (transparent).
                    // Потому что будут ещё шаблоны, накладываемые поверх всех в случайные места, чтобы генерировать дополнительные 
                    // мусорные объекты.
                    var patternValue = patternValues[x, y];
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


        private GlobeRegionPattern GetDefaultPattrn()
        {
            var defaultPatterns = new[]
            {
                GlobeRegionPatterns.Angle,
                GlobeRegionPatterns.Tringle,
                GlobeRegionPatterns.Linear,
                GlobeRegionPatterns.Diagonal
            };

            var defaultPatternIndex = _dice.Roll(0, defaultPatterns.Length - 1);
            var defaultPattern = defaultPatterns[defaultPatternIndex];
            return defaultPattern;
        }

        private void AddNode(Province region, GlobeRegionDraftValue currentPatternValue, int x, int y)
        {
            var node = CreateProvinceNodeByDraftValue(x, y, currentPatternValue);

            if (node != null)
            {
                region.AddNode(node);
            }
        }

        private ProvinceNode CreateProvinceNodeByDraftValue(int x, int y, GlobeRegionDraftValue currentPatternValue)
        {
            ProvinceNode node = null;
            if (currentPatternValue == null || currentPatternValue.Value.HasFlag(GlobeRegionDraftValueType.Wild))
            {
                // Это означает, что сюда не был применен ни один шаблон или
                // Дикий сектор был указан явно одним из шаблонов.
                // Значит генерируем просто дикий сектор.
                var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
                node = new ProvinceNode(x, y, locationScheme);
            }
            else if (currentPatternValue.IsStart)
            {
                var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
                node = new ProvinceNode(x, y, locationScheme)
                {
                    IsStart = true
                };
            }
            else if (currentPatternValue.IsHome)
            {
                var locationScheme = _schemeService.GetScheme<ILocationScheme>(CITY_SCHEME_SID);
                node = new ProvinceNode(x, y, locationScheme)
                {
                    IsTown = true,
                    IsHome = true
                };
            }
            else if (currentPatternValue.Value.HasFlag(GlobeRegionDraftValueType.Town))
            {
                var locationScheme = _schemeService.GetScheme<ILocationScheme>(CITY_SCHEME_SID);
                node = new ProvinceNode(x, y, locationScheme)
                {
                    IsTown = true
                };
            }
            else if (currentPatternValue.Value.HasFlag(GlobeRegionDraftValueType.Dungeon))
            {
                var locationSchemeSids = new[]
                {
                "rat-hole",
                "rat-kingdom",
                "demon-dungeon",
                "demon-lair",
                "crypt",
                "elder-place",
                "genomass-cave"
                };
                var locationSidIndex = _dice.Roll(0, locationSchemeSids.Length - 1);
                var locationSid = locationSchemeSids[locationSidIndex];
                var locationScheme = _schemeService.GetScheme<ILocationScheme>(locationSid);
                node = new ProvinceNode(x, y, locationScheme);
            }
            else
            {
                Debug.Assert(true, "При генерации провинции должны все исходы быть предусмотрены.");
            }

            return node;
        }

        private static bool GetIsBorder(int x, int y)
        {
            return x == 0 || x == ProvinceBaseSize - 1 || y == 0 || y == ProvinceBaseSize - 1;
        }

        private void AddBorderNode(Province region, int x, int y)
        {
            var locationScheme = _schemeService.GetScheme<ILocationScheme>(WILD_SCHEME_SID);
            
            var borderNode = new ProvinceNode(x, y, locationScheme)
            {
                IsBorder = true
            };

            region.AddNode(borderNode);
        }
    }
}
