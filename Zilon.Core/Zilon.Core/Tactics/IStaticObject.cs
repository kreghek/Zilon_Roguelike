using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    ///     Статический объект в секторе. Противоположность актёром, которые активны и воздействуют на статически-пассивные
    ///     объекты.
    /// </summary>
    public interface IStaticObject : IPassMapBlocker, IAttackTarget
    {
        /// <summary>
        ///     Идентфикиатор объекта.
        /// </summary>
        /// <remarks>
        ///     Задаётся и используется в тестах для выборки сундука.
        /// </remarks>
        int Id { get; }

        /// <summary>
        ///     Блокер проходимости карты.
        /// </summary>
        /// <remarks>
        ///     Это значение задаётся, если объект должен блокировать проходимость.
        /// </remarks>
        bool IsMapBlock { get; }

        /// <summary>
        ///     Блокер видимости.
        /// </summary>
        /// <remarks>
        ///     Это значение задаётся, если объект должен блокировать видимость.
        /// </remarks>
        bool IsSightBlock { get; }

        /// <summary>
        ///     Назначение статического объекта.
        ///     По сути - определяет внешний вид и действие по умолчанию.
        /// </summary>
        PropContainerPurpose Purpose { get; }

        /// <summary>
        ///     Получение модуля статического объекта.
        /// </summary>
        /// <typeparam name="TStaticObjectModule">Тип модуля.</typeparam>
        /// <returns>Возвращает объект модуля.</returns>
        TStaticObjectModule GetModule<TStaticObjectModule>(string key) where TStaticObjectModule : IStaticObjectModule;

        /// <summary>
        ///     Добавление модуля статического объекта.
        /// </summary>
        /// <typeparam name="TStaticObjectModule">Тип модуля.</typeparam>
        /// <param name="sectorObjectModule">Объект модуля, который нужно добавить к объекту.</param>
        void AddModule<TStaticObjectModule>(TStaticObjectModule sectorObjectModule)
            where TStaticObjectModule : IStaticObjectModule;

        /// <summary>
        ///     Проверка наличия модуля статического объекта.
        /// </summary>
        /// <returns>Возвращает true, если модуль указанного типа есть у объекта. Иначе, false.</returns>
        bool HasModule(string key);
    }
}