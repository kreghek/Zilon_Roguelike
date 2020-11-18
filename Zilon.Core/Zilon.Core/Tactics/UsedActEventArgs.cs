using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

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
        public UsedActEventArgs([NotNull] IGraphNode targetNode, [NotNull] ITacticalAct tacticalAct)
        {
            TargetNode = targetNode ?? throw new ArgumentNullException(nameof(targetNode));
            TacticalAct = tacticalAct ?? throw new ArgumentNullException(nameof(tacticalAct));
        }

        /// <summary>
        /// Совершённое над целью действие.
        /// </summary>
        [PublicAPI]
        public ITacticalAct TacticalAct { get; }

        /// <summary>
        /// Цель действия.
        /// </summary>
        [PublicAPI]
        public IGraphNode TargetNode { get; }
    }
}