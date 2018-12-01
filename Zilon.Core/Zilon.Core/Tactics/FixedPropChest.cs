using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Props;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация сундука с фиксированным лутом.
    /// </summary>
    public class FixedPropChest : ChestBase
    {
        public override bool IsMapBlock => true;

        [ExcludeFromCodeCoverage]
        public FixedPropChest(IMapNode node, IProp[] props) : this(node, props, 0)
        {

        }


        public FixedPropChest(IMapNode node, IProp[] props, int id) : base(node, new ChestStore(), id)
        {
            foreach (var prop in props)
            {
                Content.Add(prop);
            }
        }
    }
}
