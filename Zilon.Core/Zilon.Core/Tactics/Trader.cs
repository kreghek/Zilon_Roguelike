using System;
using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация торговца.
    /// </summary>
    /// <seealso cref="Zilon.Core.Tactics.ITrader" />
    public sealed class Trader : ITrader
    {
        private readonly IDropTableScheme _goodsDropTable;
        private readonly IDropResolver _dropResolver;

        /// <summary>
        /// Создаёт экземпляр торговца <see cref="Trader"/>.
        /// </summary>
        /// <param name="goodsDropTable"> Таблица дропа, на основе которой будут выдаваться товары.</param>
        /// <param name="node"> Узел карты, в котором стоит торговец. </param>
        /// <param name="dropResolver"> Сервис для работы с таблицами дропа. </param>
        /// <exception cref="ArgumentNullException">
        /// goodsDropTable
        /// or
        /// node
        /// or
        /// dropResolver
        /// </exception>
        public Trader(IDropTableScheme goodsDropTable, IMapNode node, IDropResolver dropResolver)
        {
            _goodsDropTable = goodsDropTable ?? throw new ArgumentNullException(nameof(goodsDropTable));
            Node = node ?? throw new ArgumentNullException(nameof(node));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
        }

        /// <summary>
        /// Узер карты сектора, в котором находится торговец.
        /// </summary>
        public IMapNode Node { get; }

        /// <summary>
        /// Сделка с торговцем.
        /// </summary>
        /// <returns>
        /// Возвращает товар, который отдаёт торговец.
        /// </returns>
        public IProp Offer()
        {
            var goods = _dropResolver.Resolve(new[] { _goodsDropTable });
            return goods.Single();
        }
    }
}
