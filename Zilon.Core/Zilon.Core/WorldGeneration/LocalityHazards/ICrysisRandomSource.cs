using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    public interface ICrysisRandomSource
    {
        int RollDeathPass();

        int RollDeadPopulationIndex(IEnumerable<Population> availablePopulation);
    }
}
