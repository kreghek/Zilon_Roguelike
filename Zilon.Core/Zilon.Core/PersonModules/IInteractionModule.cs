using System.Collections.Generic;

using Zilon.Core.Graphs;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Module of advanced interation with world objects.
    /// </summary>
    public interface IInteractionModule: IPersonModule
    {
        IReadOnlyCollection<IInteration> Interations { get; }
    }

    /// <summary>
    /// Common interaction with world objects
    /// </summary>
    public interface IInteration
    {
        IInteractionTarget Target { get; }
    }

    public interface IInteractionTarget
    {
        /// <summary>
        /// Target position;
        /// </summary>
        IGraphNode Node { get; }
    }
}
