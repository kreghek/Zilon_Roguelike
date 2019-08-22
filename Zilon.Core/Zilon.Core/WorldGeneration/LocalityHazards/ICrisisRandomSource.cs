using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    public interface ICrisisRandomSource
    {
        int RollDeathPass();

        int RollDeadPopulationIndex(IEnumerable<PopulationUnit> availablePopulation);
        int RollGrowth();
    }
}
