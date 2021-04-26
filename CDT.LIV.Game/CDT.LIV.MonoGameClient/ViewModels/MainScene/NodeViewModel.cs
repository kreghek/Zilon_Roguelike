
using Zilon.Core.Client;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    internal class NodeViewModel : IMapNodeViewModel
    {
        public object Item => Node;
        public HexNode Node { get; set; }
    }
}
