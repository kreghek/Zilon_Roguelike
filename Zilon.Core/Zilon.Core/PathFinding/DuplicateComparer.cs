using System.Collections.Generic;

namespace Zilon.Core.PathFinding
{
    /// <summary>
    /// System.Collections.Generic.SortedList by default does not allow duplicate items.
    /// Since items are keyed by TotalCost there can be duplicate entries per key.
    /// </summary>
    internal class DuplicateComparer : IComparer<int>
    {
        static DuplicateComparer()
        {
            Instance = new DuplicateComparer();
        }

        private DuplicateComparer()
        {
        }

        public int Compare(int x, int y)
        {
            return x <= y ? -1 : 1;
        }

        public static DuplicateComparer Instance { get; private set; }
    }
}