using System;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события при использовании действия на цель.
    /// </summary>
    public sealed class UsedActEventArgs: EventArgs
    {
        /// <summary>
        /// Цель действия.
        /// </summary>
        public IAttackTarget Target { get; }

        /// <summary>
        /// Совершённое над целью действие.
        /// </summary>
        public ITacticalAct TacticalAct { get; }

        public UsedActEventArgs(IAttackTarget target, ITacticalAct tacticalAct)
        {
            Target = target ?? throw new System.ArgumentNullException(nameof(target));
            TacticalAct = tacticalAct ?? throw new System.ArgumentNullException(nameof(tacticalAct));
        }
    }
}
