using System;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    public sealed class HungerMonitor : ICrisisMonitor
    {
        private readonly ICrisisRandomSource _crysisRandomSource;

        public HungerMonitor(ICrisisRandomSource crysisRandomSource)
        {
            _crysisRandomSource = crysisRandomSource;
        }

        public Type CrysisType => typeof(HungerCrisis);

        public ICrisis Analyze(Locality locality)
        {
            var lastIterationFood = locality.Stats.ResourcesLastIteration[LocalityResource.Food];

            locality.Stats.ResourcesStorage.TryGetValue(LocalityResource.Food, out var availableFoodFromStorage);

            var sumFood = lastIterationFood + Math.Min(availableFoodFromStorage, locality.CurrentPopulation.Count * 0.5f);

            if (sumFood < locality.CurrentPopulation.Count)
            {
                return new HungerCrisis(_crysisRandomSource);
            }

            return null;
        }
    }
}
