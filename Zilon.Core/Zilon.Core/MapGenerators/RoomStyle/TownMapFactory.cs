using System.Threading.Tasks;

using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Генератор карт для городского сеткора.
    /// </summary>
    public class TownMapFactory : IMapFactory
    {
        private const int MapSize = 10;

        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <param name="options">Параметры создания карты.</param>
        /// <returns>
        /// Возвращает экземпляр карты.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<IMap> CreateAsync(object options)
        {
            var squareMap = await SquareMapFactory.CreateAsync(MapSize);
            return squareMap;
        }
    }
}
