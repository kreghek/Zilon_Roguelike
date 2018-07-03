using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public interface ISector
    {
        void Update();

        IMapNode[] StartNodes { get; set; }
    }
}