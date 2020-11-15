using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Try to split regions to smaller.
    /// </summary>
    internal class SplitToTargetCountRegionPostProcessor : IRegionPostProcessor
    {
        private readonly IDice _dice;
        private readonly IMapRuleManager _mapRuleManager;

        public SplitToTargetCountRegionPostProcessor(IMapRuleManager mapRuleManager, IDice dice)
        {
            _mapRuleManager = mapRuleManager;
            _dice = dice;
        }

        public IEnumerable<RegionDraft> Process(IEnumerable<RegionDraft> sourceRegions)
        {
            var regionCountRule = _mapRuleManager.GetRuleOrNull<IRegionMinCountRule>();
            if (regionCountRule is null)
            {
                return sourceRegions;
            }

            var draftRegionArray = sourceRegions.ToArray();
            return SplitRegionsForTransitions(draftRegionArray, regionCountRule.Count);
        }

        /// <summary>
        /// Метод отделяет от существующих регионов ячйки таким образом,
        /// чтобы суммарно на карте число регионов равнялось числу переходов + 1 (за стартовый).
        /// Сейчас все отщеплённые регионы в первую отщепляются от произвольных.
        /// Уже одноклеточные регионы не участвуют в расщеплении.
        /// </summary>
        /// <param name="draftRegions"> Текущие регионы на карте. </param>
        /// <param name="targetRegionCount"> Целевое число регионов. </param>
        /// <returns> Возвращает новый массив черновиков регионов. </returns>
        private RegionDraft[] SplitRegionsForTransitions(
            [NotNull][ItemNotNull] RegionDraft[] draftRegions,
            int targetRegionCount)
        {
            if (draftRegions == null)
            {
                throw new ArgumentNullException(nameof(draftRegions));
            }

            if (targetRegionCount <= 0)
            {
                throw new ArgumentException("Целевое количество регионов должно быть больше 0.",
                    nameof(targetRegionCount));
            }

            var regionCountDiff = targetRegionCount - draftRegions.Length;
            if (regionCountDiff <= 0)
            {
                return (RegionDraft[])draftRegions.Clone();
            }

            var availableSplitRegions = draftRegions.Where(x => x.Coords.Count() > 1);
            var availableCoords = from region in availableSplitRegions
                                  from coord in region.Coords.Skip(1)
                                  select new RegionCoords(coord, region);

            if (availableCoords.Count() < regionCountDiff)
            {
                // Возможна ситуация, когда в принципе клеток меньше,
                // чем требуется регионов.
                // Даже если делать по одной клетки на регион.
                // В этом случае ничего сделать нельзя.
                // Передаём проблему вызывающему коду.
                throw new CellularAutomatonException(
                    "Невозможно расщепить регионы на достаточное количество. Клеток меньше, чем требуется.");
            }

            var openRegionCoords = new List<RegionCoords>(availableCoords);
            var usedRegionCoords = new List<RegionCoords>();

            for (var i = 0; i < regionCountDiff; i++)
            {
                var coordRollIndex = _dice.Roll(0, openRegionCoords.Count - 1);
                var regionCoordPair = openRegionCoords[coordRollIndex];
                openRegionCoords.RemoveAt(coordRollIndex);
                usedRegionCoords.Add(regionCoordPair);
            }

            var newDraftRegionList = new List<RegionDraft>();
            var regionGroups = usedRegionCoords.GroupBy(x => x.Region)
                .ToDictionary(x => x.Key, x => x.AsEnumerable());

            foreach (var draftRegion in draftRegions)
            {
                if (regionGroups.TryGetValue(draftRegion, out var splittedRegionCoords))
                {
                    var splittedCoords = splittedRegionCoords.Select(x => x.Coords).ToArray();

                    var newCoordsOfCurrentRegion = draftRegion.Coords
                        .Except(splittedCoords)
                        .ToArray();

                    var recreatedRegionDraft = new RegionDraft(newCoordsOfCurrentRegion);
                    newDraftRegionList.Add(recreatedRegionDraft);

                    foreach (var splittedCoord in splittedCoords)
                    {
                        var newRegionDraft = new RegionDraft(new[]
                        {
                            splittedCoord
                        });
                        newDraftRegionList.Add(newRegionDraft);
                    }
                }
                else
                {
                    newDraftRegionList.Add(draftRegion);
                }
            }

            return newDraftRegionList.ToArray();
        }

        private sealed class RegionCoords
        {
            public RegionCoords(OffsetCoords coords, RegionDraft region)
            {
                Coords = coords;
                Region = region ?? throw new ArgumentNullException(nameof(region));
            }

            public OffsetCoords Coords { get; }

            public RegionDraft Region { get; }
        }
    }
}