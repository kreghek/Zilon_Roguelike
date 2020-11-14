using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    ///     Селектор фабрики карт на основе одной единственной фабрики карт.
    /// </summary>
    public class SingleMapFactorySelector : IMapFactorySelector
    {
        private readonly IMapFactory _mapFactory;

        /// <summary>
        ///     Конструктор селектора.
        /// </summary>
        /// <param name="mapFactory"> Фабрика карт, которая всегда будет возвращена при запросе. </param>
        public SingleMapFactorySelector(IMapFactory mapFactory)
        {
            _mapFactory = mapFactory ?? throw new System.ArgumentNullException(nameof(mapFactory));
        }

        /// <inheritdoc />
        public IMapFactory GetMapFactory(ISectorNode sectorNode)
        {
            return _mapFactory;
        }
    }
}