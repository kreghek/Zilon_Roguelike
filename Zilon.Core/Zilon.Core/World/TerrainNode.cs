using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Ячейка глобального мира.
    /// </summary>
    public sealed class TerrainNode : HexNode
    {
        public TerrainNode(int x, int y, Province province) : base(x, y)
        {
            Province = province;
        }

        public Province Province { get; }
    }
}
