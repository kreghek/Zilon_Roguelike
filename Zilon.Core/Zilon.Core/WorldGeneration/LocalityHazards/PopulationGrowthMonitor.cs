using System;
using System.Linq;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    public class PopulationGrowthMonitor : ICrisisMonitor
    {
        private ICrisisRandomSource _crysisRandomSource;

        public PopulationGrowthMonitor(ICrisisRandomSource crysisRandomSource)
        {
            _crysisRandomSource = crysisRandomSource;
        }

        public Type CrysisType { get; }

        public ICrisis Analyze(Locality locality)
        {
            // Обходим все единицы населения и проверяем, нет ли среди них готовых для роста.

            var unitsForGrowth = locality.CurrentPopulation
                .Where(unit => unit.PopulationGrowthCounter >= 1)
                .ToArray();

            foreach (var unit in unitsForGrowth)
            {
                CreatePopulationUnit(locality, unit);
            }

            return null;
        }

        private void CreatePopulationUnit(Locality locality, PopulationUnit unit)
        {
            unit.DropGrowthCounter();

            var growedUnit = new PopulationUnit
            {
                Specialization = unit.Specialization
            };

            locality.CurrentPopulation.Add(growedUnit);
        }
    }
}
