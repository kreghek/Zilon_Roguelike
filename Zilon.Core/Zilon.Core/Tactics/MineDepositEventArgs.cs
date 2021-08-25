using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события на добычу залежей.
    /// </summary>
    public sealed class MineDepositEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public MineDepositEventArgs(IStaticObject deposit, [NotNull] IMineDepositResult result)
        {
            Deposit = deposit ?? throw new ArgumentNullException(nameof(deposit));
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        /// <summary>
        /// Контейнер, который пытались открыть.
        /// </summary>
        public IStaticObject Deposit { get; }

        /// <summary>
        /// Результат добычи.
        /// </summary>
        public IMineDepositResult Result { get; }
    }
}