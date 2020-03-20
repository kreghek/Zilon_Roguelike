using System;
using System.Collections.Generic;
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

                //TODO Можно запустить параллельно.
                // Но это возможно, если предварительно сделать одно из следующих действий:
                // 1. Ввесли допущение и проверку, что стартовые подземелья не появляются в узлах провинции,
                // где уже предполагаются стартовые города.
                // 2. Блокировать операцию на добавление узла. Иначе будет сгенерирован один узел дважды.
                // 3. Переписать алгоритм так, чтобы города и подземелья группировались по координатам узла.
                // Тогда можно будет последовательно создать узел и добавить в него всё, что нужно. Будет актуально,
                // когда будет снято допущение, что в одном узле может быть только один стартовый город.

                await GenerateStartLocalityProvinceNodes(globeDraft.StartLocalities, terrain).ConfigureAwait(false);

                await GenerateStartDungeonProvinceNodes(globeDraft.StartDungeons, terrain).ConfigureAwait(false);

                return terrain;
            });
        }

        private async Task GenerateStartLocalityProvinceNodes(IEnumerable<RealmLocalityDraft> startLocalities, Terrain terrain)
        {
            //TODO Можно генерировать параллельно. Не забыть делать через промежуточный ConcurrencyBag
            // Удалить этот TODO, если окажется, что параллельная генерация неэффективна.
            // Указать здесь, что попытка была сделана и она оказалась не эффетивной,
            // чтобы не появлялось таких же todo с предложением сделать параллельно.

            // Сейчас исходим из того, что в каждой провинции может быть не более одного стартового города.
            foreach (var localityDraft in startLocalities)
            {
                var province = await CreateProvinceAsync().ConfigureAwait(false);
                var terrainNode = CreateTerrainNode(localityDraft.StartTerrainCoords, province);

                terrain.AddNode(terrainNode);
            }
        }

        private async Task GenerateStartDungeonProvinceNodes(IEnumerable<DungeonDraft> startDungeons, Terrain terrain)
        {
            //TODO Можно генерировать параллельно. Не забыть делать через промежуточный ConcurrencyBag
            // Удалить этот TODO, если окажется, что параллельная генерация неэффективна.
            // Указать здесь, что попытка была сделана и она оказалась не эффетивной,
            // чтобы не появлялось таких же todo с предложением сделать параллельно.

            // Сейчас исходим из того, что в каждой провинции может быть не более одного стартового города.
            foreach (var dungeonDraft in startDungeons)
            {
                var province = await CreateProvinceAsync().ConfigureAwait(false);
                var terrainNode = CreateTerrainNode(dungeonDraft.StartTerrainCoords, province);

                terrain.AddNode(terrainNode);
            }
        }

        private static TerrainNode CreateTerrainNode(OffsetCoords coords, Province province)
        {
            var x = coords.X;
            var y = coords.Y;

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
