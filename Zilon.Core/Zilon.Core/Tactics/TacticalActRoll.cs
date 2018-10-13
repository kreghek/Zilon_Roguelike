using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Ресультат применения действия.
    /// </summary>
    public sealed class TacticalActRoll
    {
        [ExcludeFromCodeCoverage]
        public TacticalActRoll(ITacticalAct tacticalAct, int efficient)
        {
            TacticalAct = tacticalAct;
            Efficient = efficient;
        }

        /// <summary>
        /// Действие, которое было совершено.
        /// </summary>
        public ITacticalAct TacticalAct { get; }

        /// <summary>
        /// Эффективность действия.
        /// </summary>
        public int Efficient { get; }
    }
}
