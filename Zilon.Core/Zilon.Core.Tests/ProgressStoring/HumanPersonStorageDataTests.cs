using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.ProgressStoring;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.ProgressStoring
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HumanPersonStorageDataTests
    {
        [Test]
        public void Restore_MinimunPersonAfterSave_RestoredPersonEqualsOriginal()
        {
            // ARRANGE
            var tacticalActSchemes = new Dictionary<string, ITacticalActScheme>
            {
                { "default", new TestTacticalActScheme {
                    Sid = "default",
                    Stats = new TestTacticalActStatsSubScheme{
                        Efficient = new Core.Common.Roll(1, 1)
                    }
                } }
            };

            var personSchemes = new Dictionary<string, IPersonScheme>
            {
                { "human-person", new TestPersonScheme {
                    Sid = "human-person",
                    DefaultAct = "default",
                    Slots = new[] {
                        new PersonSlotSubScheme { Types = EquipmentSlotTypes.Head },
                    },
                    SurvivalStats = new[]{
                        new TestPersonSurvivalStatSubScheme
                        {
                            Type = PersonSurvivalStatType.Satiety
                        },
                        new TestPersonSurvivalStatSubScheme
                        {
                            Type = PersonSurvivalStatType.Hydration
                        }
                    }
                } }
            };

            var propSchemes = new Dictionary<string, IPropScheme>
            {
                { "helm", new TestPropScheme {
                    Sid = "helm",
                    Equip = new TestPropEquipSubScheme{
                        SlotTypes = new[]{ EquipmentSlotTypes.Head }
                    }
                }},
                { "res", new TestPropScheme {
                    Sid = "res"
                }}
            };

            var perkSchemes = new Dictionary<string, IPerkScheme>
            {
                { "perk1", new TestPerkScheme{
                    Sid = "perk1",
                    Levels = new PerkLevelSubScheme[]{
                        new PerkLevelSubScheme{
                            Jobs = new[]{
                                new TestJobSubScheme{Type = JobType.Defeats, Value = 100 }
                            }
                        }
                    }
                } }
            };

            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetScheme<IPersonScheme>(It.IsAny<string>()))
                .Returns<string>(sid => personSchemes[sid]);
            schemeServiceMock.Setup(x => x.GetScheme<ITacticalActScheme>(It.IsAny<string>()))
                .Returns<string>(sid => tacticalActSchemes[sid]);
            schemeServiceMock.Setup(x => x.GetScheme<IPropScheme>(It.IsAny<string>()))
                .Returns<string>(sid => propSchemes[sid]);
            schemeServiceMock.Setup(x => x.GetScheme<IPerkScheme>(It.IsAny<string>()))
                .Returns<string>(sid => perkSchemes[sid]);
            schemeServiceMock.Setup(x => x.GetSchemes<IPerkScheme>())
                .Returns(perkSchemes.Values.ToArray());
            var schemeService = schemeServiceMock.Object;

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var propFactory = new PropFactory(schemeService);

            var inventory = new Inventory();

            var evolutionData = new EvolutionData(schemeService);

            var person = new HumanPerson(personSchemes["human-person"],
                                         tacticalActSchemes["default"],
                                         evolutionData,
                                         survivalRandomSource,
                                         inventory);
            person.Survival.Stats.Single(x => x.Type == SurvivalStatType.Health).Value = 7;

            // Назначаем экипировку
            var helm = propFactory.CreateEquipment(propSchemes["helm"]);
            helm.Durable.Value = helm.Durable.Range.Max / 2;
            person.EquipmentCarrier[0] = helm;

            // Инвентарь
            var equipment1 = propFactory.CreateEquipment(propSchemes["helm"]);
            inventory.Add(equipment1);

            var equipment2 = propFactory.CreateEquipment(propSchemes["helm"]);
            equipment2.Durable.Value = helm.Durable.Range.Max / 2;
            inventory.Add(equipment2);

            var resource = propFactory.CreateResource(propSchemes["res"], 1);
            inventory.Add(resource);

            evolutionData.Perks.First().CurrentJobs.First().Progress = 13;

            var storageData = HumanPersonStorageData.Create(person);

            // Сериализуем
            var serialized = JsonConvert.SerializeObject(storageData);

            // Десериализуем
            var deserializedStorageData = JsonConvert.DeserializeObject<HumanPersonStorageData>(serialized);

            // ACT

            // Восстанавливаем
            var restoredPerson = deserializedStorageData.Restore(schemeService, survivalRandomSource, propFactory);

            // ASSERT
            restoredPerson.Should().BeEquivalentTo(person, options =>
            {
                options.Excluding(g => g.Effects);

                return options;
            });
        }
    }
}