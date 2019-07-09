namespace Zilon.Core.PersonDialogs
{
    public sealed class Dialog
    {
        public DialogNode RootNode { get; set; }

        public DialogNode[] Nodes { get; set; }

        public DialogTransition[] Transitions { get; set; }
    }
}
