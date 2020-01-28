using System;
using System.Threading.Tasks;

using Zilon.Core.World.GlobeDrafting;

namespace Zilon.Core.World
{
    public sealed class TerrainInitiator
    {
        private readonly ProvinceInitiator _provinceInitiator;

        public TerrainInitiator(ProvinceInitiator provinceInitiator)
        {
            _provinceInitiator = provinceInitiator ?? throw new ArgumentNullException(nameof(provinceInitiator));
        }

        public Task<Terrain> GenerateAsync(GlobeDraft globeDraft)
        {
            return Task.Run(async () =>
            {
                var terrain = new Terrain(globeDraft.Size);

                //TODO Можно генерировать параллельно. Не забыть делать через промежуточный ConcurrencyBag
                // Удалить этот TODO, если окажется, что параллельная генерация неэффективна.
                // Указать здесь, что попытка была сделана и она оказалась не эффетивной,
                // чтобы не появлялось таких же todo с предложением сделать параллельно.

                // Сейчас исходим из того, что в каждой провинции может быть не более одного стартового города.
                foreach (var startProvinceDraft in globeDraft.StartLocalities)
                {
                    var province = await CreateProvinceAsync().ConfigureAwait(false);
                    var terrainNode = CreateTerrainNode(startProvinceDraft, province);

                    terrain.AddNode(terrainNode);
                }

                return terrain;
            });
        }

        private static TerrainNode CreateTerrainNode(RealmLocalityDraft startProvinceDraft, Province province)
        {
            var x = startProvinceDraft.StartTerrainCoords.X;
            var y = startProvinceDraft.StartTerrainCoords.Y;

            var terrainNode = new TerrainNode(x, y, province)
            {
                Id = x * 100 + y
            };
            return terrainNode;
        }

        private async Task<Province> CreateProvinceAsync()
        {
            var region = await _provinceInitiator.GenerateProvinceAsync().ConfigureAwait(false);
            return region;
        }
    }
}
