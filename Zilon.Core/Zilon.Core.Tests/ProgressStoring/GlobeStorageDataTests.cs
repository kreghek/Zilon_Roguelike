using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Newtonsoft.Json;

using NUnit.Framework;

using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Tests.ProgressStoring
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class GlobeStorageDataTests
    {
        /// <summary>
        /// Тест проверяет, что восстановление минимального мира выполняется корректно.
        /// </summary>
        /// <remarks>
        /// На самом деле этот тест проверяет два метода - Create и Restore.
        /// 1. Сначала создаём мир.
        /// 2. Создаём модель сохранения мира.
        /// 3. Сериализуем.
        /// 4. Десериализуем.
        /// 5. Выполняем восстановление мира.
        /// 6. Проверяем, что восстановленный мир совпадает с изначальным.
        /// </remarks>
        [Test]
        [Category("integration")]
        public void Restore_MinimumGlobeAfterSave_RestoredGlobeEqualsToOriginal()
        {
            // ARRANGE
            var globe = new Globe();
            globe.Terrain = new TerrainCell[][]
            {
                new TerrainCell[] { new TerrainCell{ Coords = new OffsetCoords(0, 0)}, new TerrainCell { Coords = new OffsetCoords(1, 0) } },
                new TerrainCell[] { new TerrainCell{ Coords = new OffsetCoords(0, 1)}, new TerrainCell { Coords = new OffsetCoords(1, 1) } }
            };

            globe.Realms = new List<Realm>
            {
                new Realm{
                    Name = "realm-name",
                    Banner = new RealmBanner{ MainColor = new Color(0, 0, 0) }
                }
            };

            var flattenTerrain = globe.Terrain.SelectMany(x => x).ToArray();

            globe.Localities = new List<Locality>
            {
                new Locality()
                {
                    Name = "capital",
                    Cell = flattenTerrain.Single(x => x.Coords.X == 0 && x.Coords.Y == 0),
                    Owner = globe.Realms.First(),
                    Population = 1,
                    Branches = new Dictionary<BranchType, int>{ { BranchType.Agricultural, 1 } }
                }
            };
            globe.LocalitiesCells = globe.Localities.ToDictionary(x => x.Cell, x => x);

            globe.HomeProvince = globe.Localities.First().Cell;
            globe.StartProvince = globe.Localities.Last().Cell;

            // Создание модели хранения
            var storageData = GlobeStorageData.Create(globe);

            // Сериализуем
            var serialized = JsonConvert.SerializeObject(storageData);

            // Десериализуем
            var deserializedStorageData = JsonConvert.DeserializeObject<GlobeStorageData>(serialized);

            // ACT

            // Восстанавливаем мир
            var restoredGlobe = deserializedStorageData.Restore();

            // ASSERT
            restoredGlobe.Should().BeEquivalentTo(globe, options =>
            {
                options.Excluding(g => g.ScanResult);
                options.Excluding(g => g.cityNameGenerator);
                options.Excluding(g => g.agentNameGenerator);

                return options;
            });
        }
    }
}