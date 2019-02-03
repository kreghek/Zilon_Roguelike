using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Генератор карт для городского сеткора.
    /// </summary>
    public class TownMapFactory : IMapFactory
    {
        private const int MapSize = 20;

        /// <summary>
        /// Создание карты.
        /// </summary>
        /// <returns> Создаёт экземпляр карты. </returns>
        public IMap Create()
        {
            var squareMap = SquareMapFactory.Create(MapSize);
            return squareMap;
        }
    }
}
