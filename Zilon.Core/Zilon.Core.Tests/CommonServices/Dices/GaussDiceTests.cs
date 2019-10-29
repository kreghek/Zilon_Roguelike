using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    public class GaussDiceTests : DiceTestsBase
    {
        protected override IDice CreateDice(int seed)
        {
            return new GaussDice(seed);
        }
    }
}