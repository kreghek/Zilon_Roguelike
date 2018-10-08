namespace Zilon.Core.Common
{
    public class Roll
    {
        public Roll(int dice, int count)
        {
            Dice = dice;
            Count = count;
        }

        public int Dice { get; }
        public int Count { get; }
    }
}
