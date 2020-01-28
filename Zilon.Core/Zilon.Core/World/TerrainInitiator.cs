using System;
using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public sealed class TerrainInitiator
    {
        private readonly ProvinceInitiator _provinceInitiator;

        public TerrainInitiator(ProvinceInitiator provinceInitiator)
        {
            _provinceInitiator = provinceInitiator ?? throw new ArgumentNullException(nameof(provinceInitiator));
        }

        public int WorldSize { get; } = 40;

        public Task<Terrain> GenerateAsync()
        {
            return Task.Run(async () =>
            {
                var terrain = new Terrain(40);

                for (var i = 0; i < WorldSize; i++)
                {
                    //TODO Можно генерировать параллельно. Не забыть делать через промежуточный ConcurrencyBag
                    for (var j = 0; j < WorldSize; j++)
                    {
                        var province = await CreateProvinceAsync().ConfigureAwait(false);

                        var terrainNode = new TerrainNode(i, i, province) { 
                            Id = i * 100 + j
                        };

                        terrain.AddNode(terrainNode);
                    }
                }

                return terrain;
            });
        }

        private async Task<Province> CreateProvinceAsync()
        {
            var region = await _provinceInitiator.GenerateProvinceAsync().ConfigureAwait(false);
            return region;
        }
    }
}
