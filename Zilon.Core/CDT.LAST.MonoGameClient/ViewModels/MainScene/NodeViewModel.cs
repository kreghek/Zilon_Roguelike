using System;

using Zilon.Core.Client;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class NodeViewModel : IMapNodeViewModel
    {
        public NodeViewModel(HexNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public object Item => Node;
        public HexNode Node { get; set; }
    }
}