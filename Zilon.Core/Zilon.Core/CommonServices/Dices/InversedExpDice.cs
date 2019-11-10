namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Игральная кость, работающая по экпонециальному закону.
    /// </summary>
    public sealed class InversedExpDice : ExpDice
    {
        protected override int ProcessedRoll(int roll, int n)
        {
            var inversedRoll = n - roll + 1;
            return inversedRoll;
        }
    }
}
