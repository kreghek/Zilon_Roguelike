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
    }
}
