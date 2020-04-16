using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Graphs;
using Zilon.Core.Props;

namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    /// Реализация сундука с фиксированным лутом.
    /// </summary>
    public class FixedPropChest : ChestBase
    {
        public override bool IsMapBlock => true;

        [ExcludeFromCodeCoverage]
        public FixedPropChest(IGraphNode node, IProp[] props) : this(node, props, 0)
        {
        }

        public FixedPropChest(IGraphNode node, IProp[] props, int id) : base(new ChestStore())
        {
            if (props is null)
            {
                throw new System.ArgumentNullException(nameof(props));
            }

            foreach (var prop in props)
            {
                Content.Add(prop);
            }
        }
    }
}
