namespace Zilon.Core.PersonDialogs
{
    public static class DialogFactory
    {
        public static Dialog Create()
        {
            var dialog = new Dialog();

            var root = new DialogNode { Text = "Hi!" };
            var node1 = new DialogNode { Text = "The man says you what dialogs and quests comming soon." };
            var node2 = new DialogNode { Text = "You decided to wait. Press Close button on the top right corner." };

            var rootNode1Trans = new DialogTransition { Text = "Ask for dialogs.", StartNode = root, TargetNode = node1 };
            var rootNode2Trans = new DialogTransition { Text = "Wait.", StartNode = root, TargetNode = node2 };
            var node1ToRootTrans = new DialogTransition { Text = "Ok", StartNode = node1, TargetNode = root };

            dialog.Nodes = new[] { root, node1, node2 };
            dialog.Transitions = new[] { rootNode1Trans, rootNode2Trans, node1ToRootTrans };
            dialog.RootNode = root;

            return dialog;
        }
    }
}
