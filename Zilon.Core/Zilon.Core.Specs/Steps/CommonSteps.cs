using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tests.Common;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Steps
{
    [UsedImplicitly]
    [Binding]
    public class CommonSteps : GenericStepsBase<CommonGameActionsContext>
    {
        /// <summary>
        /// Количество миллисекунд, которые можно потратить на выполнение быстрой операции.
        /// Эта константа нужна, чтобы задавать лимит по времени. Чтобы быстрее проваливать тесты, которые "подвисают".
        /// </summary>
        private const int TEST_SHORT_OP_LIMIT_MS = 1000;

        [UsedImplicitly]
        public CommonSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [UsedImplicitly]
        [Given(@"Есть монстр класса (.*) Id:(.*) в ячейке \((.*), (.*)\)")]
        [Given(@"the monster with class (.*) and Id:(.*) in the map node \((.*), (.*)\)")]
        public void GivenMonsterWithClassAndIdInMapNode(string monsterSid, int monsterId, int x, int y)
        {
            var sector = Context.Globe.SectorNodes.First().Sector;
            Context.AddMonsterActor(monsterSid, monsterId, sector, new OffsetCoords(x, y));
        }

        [UsedImplicitly]
        [Given(@"Актёр игрока имеет Hp: (.*)")]
        public void GivenАктёрИмеетHp(int startHp)
        {
            var actor = Context.GetActiveActor();
            actor.Person.GetModule<ISurvivalModule>().SetStatForce(SurvivalStatType.Health, startHp);
        }

        [Given(@"В инвентаре у актёра есть ресурс: (.*) количество: (\d*)")]
        public void GivenВИнвентареУАктёраЕстьРесурс(string propSid, int count)
        {
            var actor = Context.GetActiveActor();
            Context.AddResourceToActor(propSid, count, actor);
        }

        [UsedImplicitly]
        [Given(@"Есть актёр игрока класса (.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьАктёрИгрокаКлассаCaptainВЯчейке(string personSid, int nodeX, int nodeY)
        {
            var sectorToAdd = Context.Globe.SectorNodes.First().Sector;
            Context.AddHumanActor(personSid, sectorToAdd, new OffsetCoords(nodeX, nodeY));
        }

        [UsedImplicitly]
        [Given(@"Есть карта размером (\d*)")]
        public async Task GivenЕстьКартаРазмеромAsync(int mapSize)
        {
            await Context.CreateGlobeAsync(mapSize).ConfigureAwait(false);
        }

        [UsedImplicitly]
        [Given(@"Есть сундук Id:(.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьСундукВЯчейке(int id, int offsetX, int offsetY)
        {
            var coords = new OffsetCoords(offsetX, offsetY);

            Context.AddChest(id, coords);
        }

        [UsedImplicitly]
        [Given(@"Между ячейками \((.*), (.*)\) и \((.*), (.*)\) есть стена")]
        public void GivenМеждуЯчейкамиИЕстьСтена(int x1, int y1, int x2, int y2)
        {
            Context.AddWall(x1, y1, x2, y2);
        }

        [UsedImplicitly]
        [Given(@"Монстр Id:(.*) имеет Hp (.*)")]
        public void GivenМонстрIdИмеетHp(int monsterId, int monsterHp)
        {
            var monster = Context.GetMonsterById(monsterId);

            monster.Person.GetModule<ISurvivalModule>().SetStatForce(SurvivalStatType.Health, monsterHp);
        }

        [UsedImplicitly]
        [Given(@"Сундук содержит Id:(.*) ресурс (.*) в количестве (.*)")]
        public void GivenСундукСодержитIdРусурсPistol(int id, string resourceSid, int count)
        {
            var player = Context.ServiceProvider.GetRequiredService<IPlayer>();
            var propFactory = Context.ServiceProvider.GetRequiredService<IPropFactory>();
            var schemeService = Context.ServiceProvider.GetRequiredService<ISchemeService>();

            var staticObjectManager = player.SectorNode.Sector.StaticObjectManager;

            var container = staticObjectManager.Items.Single(x => x.Id == id);

            var propScheme = schemeService.GetScheme<IPropScheme>(resourceSid);
            var resource = propFactory.CreateResource(propScheme, count);

            container.GetModule<IPropContainer>().Content.Add(resource);
        }

        [UsedImplicitly]
        [Given(@"Сундук содержит Id:(.*) экипировку (.*)")]
        public void GivenСундукСодержитIdЭкипировкуPistol(int id, string equipmentSid)
        {
            var player = Context.ServiceProvider.GetRequiredService<IPlayer>();
            var propFactory = Context.ServiceProvider.GetRequiredService<IPropFactory>();
            var schemeService = Context.ServiceProvider.GetRequiredService<ISchemeService>();

            var staticObjectManager = player.SectorNode.Sector.StaticObjectManager;

            var container = staticObjectManager.Items.Single(x => x.Id == id);

            var propScheme = schemeService.GetScheme<IPropScheme>(equipmentSid);
            var equipment = propFactory.CreateEquipment(propScheme);

            container.GetModule<IPropContainer>().Content.Add(equipment);
        }

        [UsedImplicitly]
        [Then(@"Актёр игрока имеет запас hp (.*)")]
        public void ThenАктёрИмеетЗадасHp(int expectedHp)
        {
            var actor = Context.GetActiveActor();
            var hpStat = actor.Person.GetModule<ISurvivalModule>().Stats.Single(x => x.Type == SurvivalStatType.Health);
            hpStat.Value.Should().Be(expectedHp);
        }

        [Then(@"В инвентаре у актёра есть ресурс: (.*) количество: (\d*)")]
        public void ThenВИнвентареУАктёраЕстьРесурсPropSidКоличествоExpectedCount(string propSid, int expectedCount)
        {
            var actor = Context.GetActiveActor();

            var propsInInventory = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
            var testedProp = propsInInventory.First(x => x.Scheme.Sid == propSid);

            testedProp.Should().BeOfType<Resource>();
            var testedResouce = (Resource)testedProp;

            testedResouce.Count.Should().Be(expectedCount);
        }

        [UsedImplicitly]
        [Then(@"В сундуке Id:(.*) нет предмета (.*)")]
        public void ThenВСундукеIdНетПредметаWater(int containerId, string resourceSid)
        {
            var player = Context.ServiceProvider.GetRequiredService<IPlayer>();
            var containerManager = player.SectorNode.Sector.StaticObjectManager;

            var container = containerManager.Items.Single(x => x.Id == containerId);
            var prop = container.GetModule<IPropContainer>().Content.CalcActualItems()
                .SingleOrDefault(x => x.Scheme.Sid == resourceSid);

            prop.Should().BeNull();
        }

        [UsedImplicitly]
        [Then(@"В сундуке Id:(.*) нет экипировки (.*)")]
        public void ThenВСундукеIdНетЭкипировкиPistol(int id, string propSid)
        {
            var player = Context.ServiceProvider.GetRequiredService<IPlayer>();
            var containerManager = player.SectorNode.Sector.StaticObjectManager;

            var container = containerManager.Items.Single(x => x.Id == id);
            var prop = container.GetModule<IPropContainer>().Content.CalcActualItems()
                .SingleOrDefault(x => x.Scheme.Sid == propSid);

            prop.Should().BeNull();
        }

        [UsedImplicitly]
        [Then(@"Предмет (.*) отсутствует в инвентаре актёра")]
        public void ThenЕдаСырОтсутствуетВИнвентареПерсонажа(string propSid)
        {
            var actor = Context.GetActiveActor();

            var propsInInventory = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
            var testedProp = propsInInventory.FirstOrDefault(x => x.Scheme.Sid == propSid);

            testedProp.Should().BeNull();
        }

        [UsedImplicitly]
        [Then(@"Монстр Id:(\d+) имеет Hp (\d+)")]
        public void ThenМонстрIdИмеетHp(int monsterId, int expectedMonsterHp)
        {
            var monster = Context.GetMonsterById(monsterId);
            var hpStat = monster.Person.GetModule<ISurvivalModule>().Stats
                .Single(x => x.Type == SurvivalStatType.Health);
            hpStat.Value.Should().Be(expectedMonsterHp);
        }

        [Then(@"Параметр (.*) равен (.*)")]
        public void ThenПараметр_Равен(string paramType, string paramValue)
        {
            // пока нет предметов, которые изменяют характеристики, этот метод не реализуем.
            // оставляем, чтобы после остались проверки.
        }

        [UsedImplicitly]
        [Then(@"У актёра в инвентаре есть (.*)")]
        public void ThenУАктёраВИнвентареЕстьPistol(string equipmentSchemeSid)
        {
            var actor = Context.GetActiveActor();

            var inventoryModule = actor.Person.GetModule<IInventoryModule>();
            var inventoryItems = inventoryModule.CalcActualItems();
            var foundEquipment = inventoryItems.SingleOrDefault(x => x.Scheme.Sid == equipmentSchemeSid);

            foundEquipment.Should().NotBeNull();
        }

        [When(@"Жду (.*) единиц времени")]
        public Task WhenЖдуЕдиницВремениAsync(int timeUnitCount)
        {
            return WhenСледующаяИтерацияСектораAsync(timeUnitCount);
        }

        [UsedImplicitly]
        [When(@"Следующая итерация сектора (\d+) раз")]
        public async Task WhenСледующаяИтерацияСектораAsync(int count)
        {
            var globe = Context.Globe;
            var humanTaskSource = Context.ServiceProvider
                .GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();

            var counter = count;

            var survivalModule = playerState.ActiveActor?.Actor.Person?.GetModuleSafe<ISurvivalModule>();

            // Do iteration while:
            // 1. Player actor is set in playerState and wait until he can set intention.
            // 2. Player actor is not set. It means test runs without player person. Just wait for counter.

            var testHasPlayerPerson = playerState.ActiveActor?.Actor != null;
            if (testHasPlayerPerson)
            {
                while (IsPlayerPersonCanIntent(humanTaskSource, survivalModule) && counter > 0)
                {
                    await globe.UpdateAsync().TimeoutAfter(TEST_SHORT_OP_LIMIT_MS).ConfigureAwait(false);
                    counter--;
                }
            }
            else
            {
                while (counter > 0)
                {
                    await globe.UpdateAsync().TimeoutAfter(TEST_SHORT_OP_LIMIT_MS).ConfigureAwait(false);
                    counter--;
                }
            }
        }

        [UsedImplicitly]
        [When(@"Я выбираю сундук Id:(.*)")]
        public void WhenЯВыбираюСундукId(int id)
        {
            var player = Context.ServiceProvider.GetRequiredService<IPlayer>();
            var staticObjectManager = player.SectorNode.Sector.StaticObjectManager;
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();

            var container = staticObjectManager.Items.Single(x => x.Id == id);

            var chestViewMdel = new TestContainerViewModel
            {
                StaticObject = container
            };

            playerState.HoverViewModel = chestViewMdel;
        }

        [UsedImplicitly]
        [When(@"Я выбираю ячейку \((.*), (.*)\)")]
        public void WhenЯВыбираюЯчейку(int x, int y)
        {
            Context.ClickOnNode(x, y);
        }

        [When(@"Я выполняю простой")]
        public void WhenЯВыполняюПростой()
        {
            var idleCommand = Context.ServiceProvider.GetRequiredService<NextTurnCommand>();
            idleCommand.Execute();
        }

        [When(@"Я жду (.*) итераций")]
        [When(@"В мире проходит (.*) итераций")]
        public async Task WhenЯЖдуЕдиницВремениAsync(int timeUnitCount)
        {
            var globe = Context.Globe;
            var humatTaskSource = Context.ServiceProvider
                .GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();

            var counter = timeUnitCount;

            var survivalModule = playerState.ActiveActor?.Actor.Person?.GetModuleSafe<ISurvivalModule>();

            var isPlayerActor = playerState.ActiveActor?.Actor != null;
            if (isPlayerActor)
            {
                while (counter > 0)
                {
                    for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
                    {
                        if (humatTaskSource.CanIntent() && survivalModule?.IsDead == false)
                        {
                            WhenЯВыполняюПростой();
                        }

                        await globe.UpdateAsync().TimeoutAfter(TEST_SHORT_OP_LIMIT_MS).ConfigureAwait(false);
                    }

                    counter--;
                }
            }
            else
            {
                while (counter > 0)
                {
                    for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
                    {
                        await globe.UpdateAsync().TimeoutAfter(TEST_SHORT_OP_LIMIT_MS).ConfigureAwait(false);
                    }

                    counter--;
                }
            }
        }

        [UsedImplicitly]
        [When(@"Я забираю из сундука рерурс (.*) в количестве (.*)")]
        public void WhenЯЗабираюИзСундукаРерурсWaterВКоличестве(string resourceSid, int count)
        {
            var propFactory = Context.ServiceProvider.GetRequiredService<IPropFactory>();
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();
            var propTransferCommand = Context.ServiceProvider.GetRequiredService<PropTransferCommand>();

            var actor = Context.GetActiveActor();
            var container = ((IContainerViewModel)playerState.HoverViewModel).StaticObject;

            var transferMachine = new PropTransferMachine(actor.Person.GetModule<IInventoryModule>(),
                container.GetModule<IPropContainer>().Content);
            propTransferCommand.TransferMachine = transferMachine;

            var resource = container.GetModule<IPropContainer>().Content.CalcActualItems()
                .OfType<Resource>()
                .Single(x => x.Scheme.Sid == resourceSid);

            Resource takenResource;
            if (resource.Count > count)
            {
                takenResource = propFactory.CreateResource(resource.Scheme, count);
            }
            else
            {
                takenResource = resource;
            }

            transferMachine.TransferProp(takenResource,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);

            propTransferCommand.Execute();
        }

        [UsedImplicitly]
        [When(@"Я забираю из сундука экипировку (.*)")]
        public void WhenЯЗабираюИзСундукаЭкипировкуPistol(string equipmentSchemeSid)
        {
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();
            var propTransferCommand = Context.ServiceProvider.GetRequiredService<PropTransferCommand>();

            var actor = Context.GetActiveActor();
            var container = ((IContainerViewModel)playerState.HoverViewModel).StaticObject;

            var transferMachine = new PropTransferMachine(actor.Person.GetModule<IInventoryModule>(),
                container.GetModule<IPropContainer>().Content);
            propTransferCommand.TransferMachine = transferMachine;

            var equipment = container.GetModule<IPropContainer>().Content.CalcActualItems()
                .Single(x => x.Scheme.Sid == equipmentSchemeSid);

            transferMachine.TransferProp(equipment,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);

            propTransferCommand.Execute();
        }

        private static bool IsPlayerPersonCanIntent(
            [NotNull] IHumanActorTaskSource<ISectorTaskSourceContext> humanTaskSource,
            [CanBeNull] ISurvivalModule survivalModule)
        {
            if (humanTaskSource is null)
            {
                throw new ArgumentNullException(nameof(humanTaskSource));
            }

            return !humanTaskSource.CanIntent() && survivalModule?.IsDead == false;
        }
    }
}