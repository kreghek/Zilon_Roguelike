namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема предмета.
    /// </summary>
    public interface IPropScheme : IScheme, IMimicScheme
    {
        /// <summary>
        /// Информация о предмете, как он используется для выполнения действий.
        /// </summary>
        IPropBulletSubScheme Bullet { get; }

        /// <summary>
        /// Информация о крафте данного прдмета.
        /// </summary>
        CraftSubScheme Craft { get; }

        /// <summary>
        /// Информация о том, как предмет можно экипировать.
        /// </summary>
        IPropEquipSubScheme Equip { get; }

        /// <summary>
        /// Теги предмета. Используются для описания и для некоторых правил.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "CA1819:Properties should not return arrays",
            Justification = "Свойство нужно для десериализации")]
        string[] Tags { get; }

        /// <summary>
        /// Информация о том, что будет, если предмет употребить
        /// (сьесть, выпить, использовать).
        /// </summary>
        IPropUseSubScheme Use { get; }
    }
}