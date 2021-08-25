using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Ресультат применения действия.
    /// </summary>
    public sealed class CombatActRoll
    {
        [ExcludeFromCodeCoverage]
        public CombatActRoll(ICombatAct tacticalAct, int efficient)
        {
            CombatAct = tacticalAct;
            Efficient = efficient;
        }

        /// <summary>
        /// Действие, которое было совершено.
        /// </summary>
        public ICombatAct CombatAct { get; }

        /// <summary>
        /// Эффективность действия.
        /// </summary>
        public int Efficient { get; }
    }
}