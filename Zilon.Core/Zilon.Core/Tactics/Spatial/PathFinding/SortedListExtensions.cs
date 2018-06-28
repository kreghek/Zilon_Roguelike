using System.Collections.Generic;

namespace Zilon.Core.Tactics.Spatial.PathFinding
{
    /// <summary>
	/// Extension methods to make the System.Collections.Generic.SortedList easier to use.
	/// </summary>
	internal static class SortedListExtensions
    {
        /// <summary>
        /// Checks if the SortedList is empty.
        /// </summary>
        /// <param name="sortedList">SortedList to check if it is empty.</param>
        /// <returns>True if sortedList is empty, false if it still has elements.</returns>
        internal static bool IsEmpty<TKey, TValue>(this SortedList<TKey, TValue> sortedList)
        {
            return sortedList.Count == 0;
        }

        /// <summary>
        /// Adds a INode to the SortedList.
        /// </summary>
        /// <param name="sortedList">SortedList to add the node to.</param>
        /// <param name="node">Node to add to the sortedList.</param>
        /// <param name="data"> Данные узла для алгоритма. </param>
        internal static void Add(this SortedList<int, IMapNode> sortedList, IMapNode node, AStarData data)
        {
            sortedList.Add(data.TotalCost, node);
        }

        /// <summary>
        /// Removes the node from the sorted list with the smallest TotalCost and returns that node.
        /// </summary>
        /// <param name="sortedList">SortedList to remove and return the smallest TotalCost node.</param>
        /// <returns>Node with the smallest TotalCost.</returns>
        internal static IMapNode Pop(this SortedList<int, IMapNode> sortedList)
        {
            var top = sortedList.Values[0];
            sortedList.RemoveAt(0);
            return top;
        }
    }
}
