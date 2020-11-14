namespace Zilon.TextClient
{
    internal class NodeViewModel : IMapNodeViewModel
    {
        public HexNode Node { get; set; }

        public object Item => Node;
    }
}