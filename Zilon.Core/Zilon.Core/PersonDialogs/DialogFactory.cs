namespace Zilon.Core.PersonDialogs
{
    public static class DialogFactory
    {
        public static Dialog Create()
        {
            var dialog = new Dialog();

            var root = new DialogNode { Text = "root" };
            var node1 = new DialogNode { Text = "node1" };
            var node2 = new DialogNode { Text = "node2" };

            var rootNode1Trans = new DialogTransition { Text = "To node1", StartNode = root, TargetNode = node1 };
            var rootNode2Trans = new DialogTransition { Text = "To node2", StartNode = root, TargetNode = node2 };
            var node1ToRootTrans = new DialogTransition { Text = "To node2", StartNode = node1, TargetNode = root };

            dialog.Nodes = new[] { root, node1, node2 };
            dialog.Transitions = new[] { rootNode1Trans, rootNode2Trans, node1ToRootTrans };
            dialog.RootNode = root;

            return dialog;
        }
    }
}
