using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    public class LinearDiceTests: DiceTestsBase
    {
        protected override IDice CreateDice(int seed)
        {
            return new LinearDice(seed);
        }
    }
}