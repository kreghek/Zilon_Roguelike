using Zilon.Core.Props;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Торговец.
    /// Пока самая примитивная реализация, основанная на обмене любого предмета
    /// на случайный предмет из списка торговца.
    /// Каждый торговец будет иметь тоблицу дропа с товарами, которые он предоставляет:
    /// - еда.
    /// - вода.
    /// - медикаменты.
    /// - броня.
    /// - оружие.
    /// - расходники (3 разных торговца на пули, стрелы, ману).
    /// Алгорим простой. Изымаем указанный предмет у игрока, возвращаем случайный предмет торговца.
    /// </summary>
    /// <seealso cref="Zilon.Core.Tactics.Spatial.IPassMapBlocker" />
    public interface ITrader: IPassMapBlocker
    {
        /// <summary>
        /// Узер карты сектора, в котором находится торговец.
        /// </summary>
        IMapNode Node { get; }

        /// <summary>
        /// Сделка с торговцем.
        /// </summary>
        /// <returns> Возвращает товар, который отдаёт торговец. </returns>
        IProp Offer();
    }
}
