using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    public class TwoStepMiddleDiceTests : DiceTestsBase
    {
        protected override IDice CreateDice(int seed)
        {
            return new TwoStepMiddleDice(seed);
        }
    }
}