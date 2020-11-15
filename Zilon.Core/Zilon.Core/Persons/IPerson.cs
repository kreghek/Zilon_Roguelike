using Zilon.Core.PersonModules;

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
        IFraction Fraction { get; }

        int Id { get; set; }

        PhysicalSizePattern PhysicalSize { get; }

        /// <summary>
        /// Добавление модуля статического объекта.
        /// </summary>
        /// <typeparam name="TPersonModule">Тип модуля.</typeparam>
        /// <param name="sectorObjectModule">Объект модуля, который нужно добавить к объекту.</param>
        void AddModule<TPersonModule>(TPersonModule sectorObjectModule) where TPersonModule : IPersonModule;

        /// <summary>
        /// Получение модуля статического объекта.
        /// </summary>
        /// <typeparam name="TPersonModule">Тип модуля.</typeparam>
        /// <returns>Возвращает объект модуля.</returns>
        TPersonModule GetModule<TPersonModule>(string key) where TPersonModule : IPersonModule;

        /// <summary>
        /// Проверка наличия модуля статического объекта.
        /// </summary>
        /// <returns>Возвращает true, если модуль указанного типа есть у объекта. Иначе, false.</returns>
        bool HasModule(string key);
    }
}