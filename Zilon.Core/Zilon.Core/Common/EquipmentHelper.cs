using System.Linq;

namespace Zilon.Core.Common
{
    public static class EquipmentHelper
    {
        /// <summary>
        /// Check equipment has all required tags.
        /// There is faster method.
        /// https://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order
        /// </summary>
        public static bool HasAllTags(string[] equipmentTags, string[] requiredTags)
        {
            var requiredTagsIsNotInEquipment = requiredTags.Except(equipmentTags);
            var hasAllTags = !requiredTagsIsNotInEquipment.Any();
            return hasAllTags;
        }
    }
}