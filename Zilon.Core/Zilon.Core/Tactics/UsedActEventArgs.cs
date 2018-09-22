using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    //TODO Нужен тест, который проверяет, что событие на использование выстреливает и значения этого объекта корректны.
    /// <summary>
    /// Аргументы события при использовании действия на цель.
    /// </summary>
    public sealed class UsedActEventArgs: EventArgs
    {
        /// <summary>
        /// Цель действия.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public IAttackTarget Target { get; }

        /// <summary>
        /// Совершённое над целью действие.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public ITacticalAct TacticalAct { get; }

        [ExcludeFromCodeCoverage]
        public UsedActEventArgs(IAttackTarget target, ITacticalAct tacticalAct)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            TacticalAct = tacticalAct ?? throw new ArgumentNullException(nameof(tacticalAct));
        }
    }
}
