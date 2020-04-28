using Zilon.Core.PersonModules;
using Zilon.Core.Props;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс персонажа.
    /// </summary>
    /// <remarks>
    /// Персонаж - это описание игрового объекта за пределами тактических боёв.
    /// </remarks>
    public interface IPerson
    {
        int Id { get; set; }

        PhysicalSize PhysicalSize { get; }

        /// <summary>
        /// Носитель экипировки.
        /// </summary>
        IEquipmentCarrier EquipmentCarrier { get; }

        /// <summary>
        /// Носитель тактических действий.
        /// </summary>
        ITacticalActCarrier TacticalActCarrier { get; }

        /// <summary>
        /// Данные о развитие персонажа.
        /// </summary>
        IEvolutionData EvolutionData { get; }

        /// <summary>
        /// Характеристики, используемые персонажем в бою.
        /// </summary>
        ICombatStats CombatStats { get; }

        /// <summary>
        /// Инвентарь персонажа.
        /// </summary>
        /// <remarks>
        /// Для монстров равен null.
        /// </remarks>
        IPropStore Inventory { get; }

        /// <summary>
        /// Признак того, что персонаж может иметь инвентарь.
        /// Для монстров - false.
        /// </summary>
        bool HasInventory { get; }

        /// <summary>
        /// Данные по выживанию персонажа.
        /// </summary>
        ISurvivalData Survival { get; }

        EffectCollection Effects { get; }

        /// <summary>
        /// Модуль заболеваний персонажа.
        /// </summary>
        IDiseaseData DiseaseData { get; }

        /// <summary>
        /// Получение модуля статического объекта.
        /// </summary>
        /// <typeparam name="TStaticObjectModule">Тип модуля.</typeparam>
        /// <returns>Возвращает объект модуля.</returns>
        TStaticObjectModule GetModule<TStaticObjectModule>(string key) where TStaticObjectModule : IPersonModule;

        /// <summary>
        /// Добавление модуля статического объекта.
        /// </summary>
        /// <typeparam name="TStaticObjectModule">Тип модуля.</typeparam>
        /// <param name="sectorObjectModule">Объект модуля, который нужно добавить к объекту.</param>
        void AddModule<TStaticObjectModule>(TStaticObjectModule sectorObjectModule) where TStaticObjectModule : IPersonModule;

        /// <summary>
        /// Проверка наличия модуля статического объекта.
        /// </summary>
        /// <returns>Возвращает true, если модуль указанного типа есть у объекта. Иначе, false.</returns>
        bool HasModule(string key);
    }
}