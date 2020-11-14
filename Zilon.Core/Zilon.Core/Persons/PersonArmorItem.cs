using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    /// <summary>
    ///     Элемент характеристик брони персонажа.
    /// </summary>
    public class PersonArmorItem
    {
        [ExcludeFromCodeCoverage]
        public PersonArmorItem(ImpactType impact, PersonRuleLevel absorbtionLevel, int armorRank)
        {
            Impact = impact;
            AbsorbtionLevel = absorbtionLevel;
            ArmorRank = armorRank;
        }

        /// <summary>
        ///     Тип воздействия, которое выдерживает броня.
        /// </summary>
        public ImpactType Impact { get; }

        /// <summary>
        ///     Уровень поглощения урона бронёй.
        /// </summary>
        public PersonRuleLevel AbsorbtionLevel { get; }

        /// <summary>
        ///     Ранг брони.
        /// </summary>
        public int ArmorRank { get; }
    }
}