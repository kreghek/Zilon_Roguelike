using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Graphs;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события при использовании действия на цель.
    /// </summary>
    public sealed class UsedActEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public UsedActEventArgs([NotNull] IGraphNode targetNode, [NotNull] ICombatAct tacticalAct)
        {
            TargetNode = targetNode ?? throw new ArgumentNullException(nameof(targetNode));
            TacticalAct = tacticalAct ?? throw new ArgumentNullException(nameof(tacticalAct));
        }

        /// <summary>
        /// Совершённое над целью действие.
        /// </summary>
        public ICombatAct TacticalAct { get; }

        /// <summary>
        /// Цель действия.
        /// </summary>
        public IGraphNode TargetNode { get; }
    }
}