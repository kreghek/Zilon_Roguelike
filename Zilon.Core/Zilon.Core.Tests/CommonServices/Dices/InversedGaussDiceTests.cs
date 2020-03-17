using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    public class InversedGaussDiceTests : DiceTestsBase
    {
        protected override IDice CreateDice(int seed)
        {
            return new InversedGaussDice(seed);
        }
    }
}