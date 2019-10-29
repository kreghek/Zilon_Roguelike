using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Селектор фабрики карт на основе одной единственной фабрики карт.
    /// </summary>
    public class SingleMapFactorySelector : IMapFactorySelector
    {
        private readonly IMapFactory _mapFactory;

        /// <summary>
        /// Конструктор селектора.
        /// </summary>
        /// <param name="mapFactory"> Фабрика карт, которая всегда будет возвращена при запросе. </param>
        public SingleMapFactorySelector(IMapFactory mapFactory)
        {
            _mapFactory = mapFactory ?? throw new System.ArgumentNullException(nameof(mapFactory));
        }

        /// <summary>
        /// Вернуть фабрику карт.
        /// </summary>
        /// <param name="sectorScheme">Схема сектора. не используется в данном селекторе.</param>
        /// <returns> Возаращает экземпляр фабрики карт. </returns>
        public IMapFactory GetMapFactory(ISectorSubScheme sectorScheme)
        {
            return _mapFactory;
        }
    }
}
