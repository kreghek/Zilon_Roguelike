using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <inheritdoc cref="IPropScheme" />
    /// <summary>
    /// Схема предмета. Общая для всех предметов в игре.
    /// </summary>
    public class PropScheme : SchemeBase, IPropScheme
    {
        public PropScheme()
        {
        }

        [UsedImplicitly]
        [JsonConstructor]
        public PropScheme(PropEquipSubScheme equip, PropUseSubScheme use, CraftSubScheme craft)
        {
            Equip = equip;
            Use = use;
            Craft = craft;
        }

        /// <summary>
        /// Характеристики схемы, связанные с экипировкой предмета персонажем.
        /// </summary>
        public IPropEquipSubScheme Equip { get; set; }

        /// <summary>
        /// Информация об использовании предмета.
        /// </summary>
        public IPropUseSubScheme Use { get; set; }

        /// <summary>
        /// Информации о создании/разборе предмета.
        /// </summary>
        public CraftSubScheme Craft { get; }
    }
}
