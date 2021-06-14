using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Structure to pass act context to use action service.
    /// </summary>
    public class ActTargetInfo
    {
        [ExcludeFromCodeCoverage]
        public ActTargetInfo(IAttackTarget targetObject, IGraphNode targetNode)
        {
            TargetObject = targetObject ?? throw new ArgumentNullException(nameof(targetObject));
            TargetNode = targetNode ?? throw new ArgumentNullException(nameof(targetNode));
        }

        [ExcludeFromCodeCoverage]
        public IGraphNode TargetNode { get; }

        [ExcludeFromCodeCoverage]
        public IAttackTarget TargetObject { get; }
    }
}