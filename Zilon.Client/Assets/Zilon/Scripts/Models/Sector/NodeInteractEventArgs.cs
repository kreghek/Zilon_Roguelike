using System;

namespace Assets.Zilon.Scripts.Models.Sector
{
    public sealed class NodeInteractEventArgs: EventArgs
    {
        public NodeInteractEventArgs(MapNodeVM nodeViewModel)
        {
            NodeViewModel = nodeViewModel ?? throw new ArgumentNullException(nameof(nodeViewModel));
        }

        public MapNodeVM NodeViewModel { get; }
    }
}
