using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события на добычу залежей.
    /// </summary>
    public sealed class MineDepositEventArgs : EventArgs
    {
        /// <summary>
        /// Контейнер, который пытались открыть.
        /// </summary>
        [PublicAPI]
        public IStaticObject Deposit { get; }

        /// <summary>
        /// Результат добычи.
        /// </summary>
        [PublicAPI]
        public IMineDepositResult Result { get; }

        [ExcludeFromCodeCoverage]
        public MineDepositEventArgs(IStaticObject deposit, [NotNull] IMineDepositResult result)
        {
            Deposit = deposit ?? throw new ArgumentNullException(nameof(deposit));
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
    }
}
