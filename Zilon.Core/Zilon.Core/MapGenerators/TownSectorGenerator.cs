using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Генератор сектора для городской локации.
    /// </summary>
    public sealed class TownSectorGenerator : ITownSectorGenerator
    {
        private readonly IMapFactory _mapFactory;
        private readonly ISectorFactory _sectorFactory;

        /// <summary>
        /// Конструктор генератора сектора.
        /// </summary>
        /// <param name="mapFactory"> Фабрика карты. Сюда будет передаваться <see cref="TownMapFactory"/> </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        public TownSectorGenerator(
            IMapFactory mapFactory,
            ISectorFactory sectorFactory
            )
        {
            _mapFactory = mapFactory;
            _sectorFactory = sectorFactory;
        }

        /// <summary>
        /// Создание сектора.
        /// </summary>
        /// <param name="options"> Настройки генерации сектора. </param>
        /// <returns> Возвращает экземпляр сектора. </returns>
        public ISector Generate(ISectorGeneratorOptions options)
        {
            var map = _mapFactory.Create();

            var sector = _sectorFactory.Create(map);

            return sector;
        }
    }
}
