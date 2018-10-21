using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    public class PersonArmorItem
    {
        public PersonArmorItem(ImpactType impact, PersonRuleLevel absorbtionLevel, int armorRank)
        {
            Impact = impact;
            AbsorbtionLevel = absorbtionLevel;
            ArmorRank = armorRank;
        }

        public ImpactType Impact { get; }

        /// <summary>
        /// Уровень поглощения урона бронё
        /// </summary>
        public PersonRuleLevel AbsorbtionLevel { get; }

        /// <summary>
        /// Ранг брони.
        /// </summary>
        public int ArmorRank { get; }
    }
}
