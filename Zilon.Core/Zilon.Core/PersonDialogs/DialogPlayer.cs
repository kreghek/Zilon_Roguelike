using System.Linq;

namespace Zilon.Core.PersonDialogs
{
    public static class DialogPlayer
    {
        public static DialogNode SelectNode(Dialog dialog, DialogNode currentNode, DialogTransition selectedTransition)
        {
            var transitionNode = selectedTransition.TargetNode;

            return transitionNode;
        }

        public static DialogTransition[] GetAvailableTransitions(Dialog dialog, DialogNode currentNode)
        {
            var node = dialog.RootNode;

            if (currentNode != null)
            {
                node = currentNode;
            }

            return dialog.Transitions.Where(x => x.StartNode == currentNode).ToArray();
        }
    }
}
