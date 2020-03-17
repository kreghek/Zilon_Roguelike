using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    public class InversedExpDiceTests : DiceTestsBase
    {
        protected override IDice CreateDice(int seed)
        {
            return new InversedExpDice(seed);
        }
    }
}