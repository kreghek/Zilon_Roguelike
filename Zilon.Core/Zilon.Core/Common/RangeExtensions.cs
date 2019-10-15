using System;

namespace Zilon.Core.Common
{
    public static class RangeExtensions
    {
        public static T GetBounded<T>(this Range<T> range, T value) where T : IComparable
        {
            if (range is null)
            {
                throw new ArgumentNullException(nameof(range));
            }

            var cMin = value.CompareTo(range.Min);
            if (cMin <= -1)
            {
                return range.Min;
            }

            var cMax = value.CompareTo(range.Max);
            if (cMax >= 1)
            {
                return range.Max;
            }

            return value;
        }
    }
}
