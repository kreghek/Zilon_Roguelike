using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события при использовании действия на цель.
    /// </summary>
    public sealed class UsedActEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public UsedActEventArgs([NotNull] IAttackTarget target, [NotNull] ITacticalAct tacticalAct)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            TacticalAct = tacticalAct ?? throw new ArgumentNullException(nameof(tacticalAct));
        }

        /// <summary>
        /// Цель действия.
        /// </summary>
        [PublicAPI]
        public IAttackTarget Target { get; }

        /// <summary>
        /// Совершённое над целью действие.
        /// </summary>
        [PublicAPI]
        public ITacticalAct TacticalAct { get; }
    }
}