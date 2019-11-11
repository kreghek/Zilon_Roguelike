using System;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    public interface ICrisisMonitor
    {
        ICrisis Analyze(Locality locality);

        Type CrysisType { get; }
    }
}
