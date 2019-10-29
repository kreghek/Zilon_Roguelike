using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    public class ExpDiceTests : DiceTestsBase
    {
        protected override IDice CreateDice(int seed)
        {
            return new ExpDice(seed);
        }
    }
}