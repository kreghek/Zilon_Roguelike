using System;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    public interface ICrysisMonitor
    {
        ICrysis Analyze(Locality locality);

        Type CrysisType { get; }
    }
}
