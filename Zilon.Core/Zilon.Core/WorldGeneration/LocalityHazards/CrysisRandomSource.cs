using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    public sealed class CrysisRandomSource : ICrisisRandomSource
    {
        private readonly IDice _dice;

        public CrysisRandomSource(IDice dice)
        {
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public int RollDeadPopulationIndex(IEnumerable<PopulationUnit> availablePopulation)
        {
            return _dice.RollArrayIndex(availablePopulation.ToArray());
        }

        public int RollDeathPass()
        {
            return _dice.Roll2D6();
        }

        public int RollGrowth()
        {
            return _dice.Roll2D6();
        }
    }
}
