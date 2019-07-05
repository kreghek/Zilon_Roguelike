using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.Core.PersonDialogs
{
    public static class DialogPlayer
    {
        public static DialogNode SelectNode(Dialog dialog, DialogNode currentNode, DialogTransition selectedTransition)
        {
            if (currentNode == null)
            {
                return dialog.RootNode;
            }

            var transitionNode = selectedTransition.TargetNode;

            return transitionNode;
        }

        public static DialogNode[] GetAvailableTransitions(Dialog dialog, DialogNode currentNode)
        {
            var node = dialog.RootNode;

            DialogNode[] nodes = null;

            if (currentNode != null)
            {
                node = currentNode;
            }

            return dialog.Transitions.Where(x => x.StartNode == currentNode).Select(x => x.TargetNode).ToArray();
        }
    }
}
