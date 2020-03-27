using System.Collections.Generic;

using FluentAssertions;

using Newtonsoft.Json;

using NUnit.Framework;

using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.ProgressStoring.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HumanPlayerStorageDataTests
    {
        [Test]
        [Category("integration")]
        [Ignore("Потому что тест сейчас ничего не проверяет")]
        public void RestoreTest()
        {
            // ARRANGE

            const string SCHEME_SID = "rat-hole";

            var locationSchemes = new Dictionary<string, ILocationScheme>
            {
                { SCHEME_SID, new TestLocationScheme
                {
                    Sid = SCHEME_SID,
                    SectorLevels = new ISectorSubScheme[]
                    {
                        new TestSectorSubScheme
                        {

                        }
                    }
                } }
            };

            var locationScheme = locationSchemes[SCHEME_SID];
            var biome = new Biome(locationScheme);

            var sectorNode = new SectorNode(biome, locationScheme.SectorLevels[0]);

            var humanPlayer = new HumanPlayer();
            humanPlayer.BindSectorNode(sectorNode);

            // Создание модели хранения
            var storageData = HumanPlayerStorageData.Create(humanPlayer);

            // Сериализуем
            var serialized = JsonConvert.SerializeObject(storageData);

            // Десериализуем
            var deserializedStorageData = JsonConvert.DeserializeObject<HumanPlayerStorageData>(serialized);

            // ACT

            // Восстанавливаем мир
            var restoredPlayer = new HumanPlayer();
            deserializedStorageData.Restore(restoredPlayer);

            // ASSERT
            restoredPlayer.Should().BeEquivalentTo(humanPlayer, options =>
            {
                options.Excluding(g => g.MainPerson);

                // Этот фрагмент делает тест совершенно бесполезным
                options.Excluding(g => g.SectorNode);

                return options;
            });
        }
    }
}