namespace Zilon.Core.Schemes
{
    /// <inheritdoc />
    /// <summary>
    /// Схема предмета. Общая для всех предметов в игре.
    /// </summary>
    public class PropScheme : SchemeBase
    {
        /// <summary>
        /// Характеристики схемы, связанные с экипировкой предмета персонажем.
        /// </summary>
        public PropEquipSubScheme Equip { get; set; }

        /// <summary>
        /// Информация об использовании предмета.
        /// </summary>
        public PropUseSubScheme Use { get; set; }

        /// <summary>
        /// Информации о создании/разборе предмета.
        /// </summary>
        public CraftSubScheme Craft { get; set; }
    }
}
