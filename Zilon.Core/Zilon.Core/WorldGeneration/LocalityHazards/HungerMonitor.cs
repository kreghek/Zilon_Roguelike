using System;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    public sealed class HungerMonitor : ICrysisMonitor
    {
        private readonly ICrysisRandomSource _crysisRandomSource;

        public HungerMonitor(ICrysisRandomSource crysisRandomSource)
        {
            _crysisRandomSource = crysisRandomSource;
        }

        public Type CrysisType => typeof(HungerCrysis);

        public ICrysis Analyze(Locality locality)
        {
            var food = locality.Stats.Resources[LocalityResource.Food];

            if (food < 0)
            {
                return new HungerCrysis(_crysisRandomSource);
            }

            return null;
        }
    }
}
