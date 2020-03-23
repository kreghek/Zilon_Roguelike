using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.ProgressStoring.Tests
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class HumanPlayerStorageDataTests
    {
        [Test]
        [Category("integration")]
        public void RestoreTest()
        {
            // ARRANGE
            var schemeServiceMock = new Mock<ISchemeService>();
            var locationSchemes = new Dictionary<string, ILocationScheme>
            {
                { "city", new TestLocationScheme{
                    Sid = "city",
                    
                } },
                { "forest", new TestLocationScheme{
                    Sid = "forest",

                } }
            };
            schemeServiceMock.Setup(x => x.GetScheme<ILocationScheme>(It.IsAny<string>()))
                .Returns<string>(sid => locationSchemes[sid]);
            var schemeService = schemeServiceMock.Object;


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
                    Banner = new RealmBanner{ MainColor = new BannerColor(0, 0, 0) }
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

            var region = new GlobeRegion(20);

            var regionNode = new GlobeRegionNode(0, 1, locationSchemes["forest"]);

            region.AddNode(regionNode);

            var worldManager = new WorldManager(schemeService, new LinearDice(1));
            worldManager.Regions[globe.Localities.Last().Cell] = region;

            var humanPlayer = new HumanPlayer();
            humanPlayer.Terrain = globe.Localities.Last().Cell;
            humanPlayer.GlobeNode = regionNode;

            // Создание модели хранения
            var storageData = HumanPlayerStorageData.Create(humanPlayer);

            // Сериализуем
            var serialized = JsonConvert.SerializeObject(storageData);

            // Десериализуем
            var deserializedStorageData = JsonConvert.DeserializeObject<HumanPlayerStorageData>(serialized);



            // ACT

            // Восстанавливаем мир
            var restoredPlayer = new HumanPlayer();
            deserializedStorageData.Restore(restoredPlayer, globe, worldManager);



            // ASSERT
            restoredPlayer.Should().BeEquivalentTo(humanPlayer, options =>
            {
                options.Excluding(g => g.MainPerson);

                return options;
            });
        }
    }
}