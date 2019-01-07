using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    public class MonsterGeneratorRandomSource : IMonsterGeneratorRandomSource
    {
        private readonly IDice _dice;

        public MonsterGeneratorRandomSource(IDice dice)
        {
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public int RollCount()
        {
            return _dice.Roll(0, 5);
        }

        public IMonsterScheme RollMonsterScheme(IEnumerable<IMonsterScheme> availableMonsterSchemes)
        {
            var count = availableMonsterSchemes.Count();
            var rollIndex = _dice.Roll(0, count - 1);
            return availableMonsterSchemes.ElementAt(rollIndex);
        }

        public int RollNodeIndex(int count)
        {
            return _dice.Roll(0, count - 1);
        }

        public int RollRarity()
        {
            return _dice.Roll(0, 2);
        }
    }
}
