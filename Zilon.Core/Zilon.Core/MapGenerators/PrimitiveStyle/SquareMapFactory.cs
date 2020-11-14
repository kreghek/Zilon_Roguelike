using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.PrimitiveStyle
{
    /// <summary>
    /// Реализация фабрики для построения квадратной карты указанного размера.
    /// </summary>
    /// <seealso cref="IMapFactory" />
    public class SquareMapFactory : IMapFactory
    {
        /// <inheritdoc/>
        public Task<ISectorMap> CreateAsync(ISectorMapFactoryOptions generationOptions)
        {
            if (generationOptions is null)
            {
                throw new ArgumentNullException(nameof(generationOptions));
            }

            var factoryOptions = (ISectorSquareMapFactoryOptionsSubScheme)generationOptions.OptionsSubScheme;
            if (factoryOptions == null)
            {
                throw new ArgumentException(
                    $"Для {nameof(generationOptions)} не задано {nameof(ISectorSubScheme.MapGeneratorOptions)} равно null.");
            }

            var mapSize = factoryOptions.Size;

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

            var squaregenerationOptionsSubScheme = new SquareGenerationOptionsSubScheme { Size = mapSize };
            var generationOptions = new SectorMapFactoryOptions(squaregenerationOptionsSubScheme);

            return await factory.CreateAsync(generationOptions).ConfigureAwait(false);
        }

        private class SquareGenerationOptionsSubScheme : ISectorSquareMapFactoryOptionsSubScheme
        {
            public SchemeSectorMapGenerator MapGenerator { get; }

            public int Size { get; set; }
        }
    }
}