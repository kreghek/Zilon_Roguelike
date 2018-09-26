using System;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейт предмета в секторе.
    /// </summary>
    public interface IPropContainer: IPassMapBlocker
    {
        /// <summary>
        /// Идентфикиатор контейнера.
        /// </summary>
        /// <remarks>
        /// Задаётся и используется в тестах для выборки сундука.
        /// </remarks>
        int Id { get; }

        /// <summary>
        /// Узер карты сектора, в котором находится контейнер.
        /// </summary>
        IMapNode Node { get; }

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
        /// Блокер проходимости карты.
        /// </summary>
        /// <remarks>
        /// Это значение задаётся, если контейнер должен блокировать проходимость.
        /// </remarks>
        bool IsMapBlock { get; }

        /// <summary>
        /// Открытие контейнера.
        /// </summary>
        void Open();
    }
}
