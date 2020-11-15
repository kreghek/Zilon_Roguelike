namespace Zilon.TextClient
{
    internal class NodeViewModel : IMapNodeViewModel
    {
        public object Item => Node;

        public HexNode Node { get; set; }
    }
}