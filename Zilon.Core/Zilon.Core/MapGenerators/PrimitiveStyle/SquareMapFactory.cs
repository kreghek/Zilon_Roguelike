using System.Linq;
using System.Threading.Tasks;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.PrimitiveStyle
{
    /// <summary>
    /// Реализация фабрики для построения квадратной карты указанного размера.
    /// </summary>
    /// <seealso cref="Zilon.Core.MapGenerators.IMapFactory" />
    public class SquareMapFactory : IMapFactory
    {
        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <param name="options">Параметры создания карты.</param>
        /// <returns>
        /// Возвращает экземпляр карты.
        /// </returns>
        public Task<ISectorMap> CreateAsync(object options)
        {
            var mapSize = (int)options;

            ISectorMap map = new SectorGraphMap();
            MapFiller.FillSquareMap(map, mapSize);

            var mapRegion = new MapRegion(1, map.Nodes.ToArray())
            {
                IsStart = true,
                IsOut = true,
                ExitNodes = new[] { map.Nodes.Last() }
            };

            map.Regions.Add(mapRegion);

            return Task.FromResult(map);
        }

        /// <summary>
        /// Вспомогательный метод для создания квадратной карты без создания экземпляра фабрики.
        /// </summary>
        /// <param name="mapSize"> Размер карты. </param>
        /// <returns> Возвращает объект карты. </returns>
        public static async Task<ISectorMap> CreateAsync(int mapSize)
        {
            var factory = new SquareMapFactory();
            return await factory.CreateAsync((object)mapSize);
        }
    }
}
