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
            var food = locality.Stats.ResourcesBalance[LocalityResource.Food];

            if (food < 0)
            {
                return new HungerCrisis(_crysisRandomSource);
            }

            return null;
        }
    }
}
