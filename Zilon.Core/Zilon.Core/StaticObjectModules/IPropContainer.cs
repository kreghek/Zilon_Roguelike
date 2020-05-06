using System;

using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    /// Интерфейт предмета в секторе.
    /// </summary>
    public interface IPropContainer : IPassMapBlocker, IStaticObjectModule
    {
        /// <summary>
        /// Содержимое контейнера.
        /// </summary>
        IPropStore Content { get; }

        /// <summary>
        /// Признак того, что контейнер открыт.
        /// </summary>
        /// <remarks>
        /// Открытые контейнеры в дальнейшем можно открывать "руками".
        /// </remarks>
        bool IsOpened { get; }

        /// <summary>
        /// Событие выстреливает, когда сундук открывается.
        /// </summary>
        /// <remarks>
        /// Используется клиентом для изменения визуального вида открырых сундуков.
        /// </remarks>
        event EventHandler Opened;

        /// <summary>
        /// Выстреливает, когда в контейнер добавлен предмет.
        /// </summary>
        event EventHandler<PropStoreEventArgs> ItemsAdded;

        /// <summary>
        /// Выстреливает, когда из контейнера удалён предмет.
        /// </summary>
        event EventHandler<PropStoreEventArgs> ItemsRemoved;

        /// <summary>
        /// Открытие контейнера.
        /// </summary>
        void Open();

        /// <summary>
        /// Назначение сундука.
        /// </summary>
        PropContainerPurpose Purpose { get; }

        /// <summary>
        /// Блокер проходимости карты.
        /// </summary>
        /// <remarks>
        /// Это значение задаётся, если контейнер должен блокировать проходимость.
        /// </remarks>
        bool IsMapBlock { get; }
    }
}
