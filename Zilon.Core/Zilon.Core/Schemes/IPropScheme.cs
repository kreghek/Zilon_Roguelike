namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема предмета.
    /// </summary>
    public interface IPropScheme: IScheme
    {
        /// <summary>
        /// Теги предмета. Используются для описания и для некоторых правил.
        /// </summary>
        string[] Tags { get; }

        /// <summary>
        /// Информация о крафте данного прдмета.
        /// </summary>
        CraftSubScheme Craft { get; }

        /// <summary>
        /// Информация о том, как предмет можно экипировать.
        /// </summary>
        IPropEquipSubScheme Equip { get; }

        /// <summary>
        /// Информация о том, что будет, если предмет употребить
        /// (сьесть, выпить, использовать).
        /// </summary>
        IPropUseSubScheme Use { get; }
        
        /// <summary>
        /// Информация о предмете, как он используется для выполнения действий.
        /// </summary>
        IPropBulletSubScheme Bullet { get; }

        /// <summary>
        /// Идентфиикатор схемы другого предмета,
        /// под который будет мимикрировать данный предмет.
        /// Используется лже-предметами.
        /// </summary>
        string IsMimicFor { get; }
    }
}