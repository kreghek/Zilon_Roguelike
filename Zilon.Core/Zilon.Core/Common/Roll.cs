using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Common
{
    public class Roll
    {
        [ExcludeFromCodeCoverage]
        public Roll(int dice, int count) : this(dice, count, null)
        {
            
        }

        [ExcludeFromCodeCoverage]
        public Roll(int dice, int count, RollModifiers modifiers)
        {
            Dice = dice;
            Count = count;
            Modifiers = modifiers;
        }

        public int Dice { get; }
        public int Count { get; }
        public RollModifiers Modifiers { get; }
    }
}
