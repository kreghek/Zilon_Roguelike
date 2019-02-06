using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    /// <summary>
    /// Модель представления для торговца в секторе.
    /// Нужна для клиента.
    /// </summary>
    /// <seealso cref="Zilon.Core.Client.ISelectableViewModel" />
    public interface ITraderViewModel : ISelectableViewModel
    {
        /// <summary> Доменная модель торговца. </summary>
        ITrader Trader { get; set; }
    }
}
