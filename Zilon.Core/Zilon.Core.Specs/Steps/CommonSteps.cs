using System.Linq;

using FluentAssertions;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Specs.Steps
{
    [UsedImplicitly]
    [Binding]
    public class CommonSteps : GenericStepsBase<CommonGameActionsContext>
    {
        [UsedImplicitly]
        public CommonSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [UsedImplicitly]
        [Given(@"Есть карта размером (\d*)")]
        public async System.Threading.Tasks.Task GivenЕстьКартаРазмеромAsync(int mapSize)
        {
            await Context.CreateSectorAsync(mapSize);
        }

        [UsedImplicitly]
        [Given(@"Между ячейками \((.*), (.*)\) и \((.*), (.*)\) есть стена")]
        public void GivenМеждуЯчейкамиИЕстьСтена(int x1, int y1, int x2, int y2)
        {
            Context.AddWall(x1, y1, x2, y2);
        }

        [UsedImplicitly]
        [Given(@"Есть актёр игрока класса (.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьАктёрИгрокаКлассаCaptainВЯчейке(string personSid, int nodeX, int nodeY)
        {
            Context.AddHumanActor(personSid, new OffsetCoords(nodeX, nodeY));
        }

        [UsedImplicitly]
        [Given(@"Актёр игрока имеет Hp: (.*)")]
        public void GivenАктёрИмеетHp(int startHp)
        {
            var actor = Context.GetActiveActor();
            actor.Person.GetModule<ISurvivalModule>().SetStatForce(SurvivalStatType.Health, startHp);
        }

        [UsedImplicitly]
        [Given(@"Есть монстр класса (.*) Id:(.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьМонстрКлассаRatВЯчейке(string monsterSid, int monsterId, int x, int y)
        {
            Context.AddMonsterActor(monsterSid, monsterId, new OffsetCoords(x, y));
        }

        [UsedImplicitly]
        [Given(@"Монстр Id:(.*) имеет Hp (.*)")]
        public void GivenМонстрIdИмеетHp(int monsterId, int monsterHp)
        {
            var monster = Context.GetMonsterById(monsterId);

            monster.Person.GetModule<ISurvivalModule>().SetStatForce(SurvivalStatType.Health, monsterHp);
        }

        [UsedImplicitly]
        [Given(@"Есть сундук Id:(.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьСундукВЯчейке(int id, int offsetX, int offsetY)
        {
            var coords = new OffsetCoords(offsetX, offsetY);
            Context.AddChest(id, coords);
        }

        [UsedImplicitly]
        [Given(@"Сундук содержит Id:(.*) экипировку (.*)")]
        public void GivenСундукСодержитIdЭкипировкуPistol(int id, string equipmentSid)
        {
            var sectorManager = Context.ServiceProvider.GetRequiredService<ISectorManager>();
            var propFactory = Context.ServiceProvider.GetRequiredService<IPropFactory>();
            var schemeService = Context.ServiceProvider.GetRequiredService<ISchemeService>();

            var staticObjectManager = sectorManager.CurrentSector.StaticObjectManager;

            var container = staticObjectManager.Items.Single(x => x.Id == id);

            var propScheme = schemeService.GetScheme<IPropScheme>(equipmentSid);
            var equipment = propFactory.CreateEquipment(propScheme);

            container.GetModule<IPropContainer>().Content.Add(equipment);
        }

        [UsedImplicitly]
        [Given(@"Сундук содержит Id:(.*) ресурс (.*) в количестве (.*)")]
        public void GivenСундукСодержитIdРусурсPistol(int id, string resourceSid, int count)
        {
            var sectorManager = Context.ServiceProvider.GetRequiredService<ISectorManager>();
            var propFactory = Context.ServiceProvider.GetRequiredService<IPropFactory>();
            var schemeService = Context.ServiceProvider.GetRequiredService<ISchemeService>();

            var staticObjectManager = sectorManager.CurrentSector.StaticObjectManager;

            var container = staticObjectManager.Items.Single(x => x.Id == id);

            var propScheme = schemeService.GetScheme<IPropScheme>(resourceSid);
            var resource = propFactory.CreateResource(propScheme, count);

            container.GetModule<IPropContainer>().Content.Add(resource);
        }

        [Given(@"В инвентаре у актёра есть ресурс: (.*) количество: (\d*)")]
        public void GivenВИнвентареУАктёраЕстьРесурс(string propSid, int count)
        {
            var actor = Context.GetActiveActor();
            Context.AddResourceToActor(propSid, count, actor);
        }

        [UsedImplicitly]
        [When(@"Следующая итерация сектора")]
        public void WhenСледующаяИтерацияСектора()
        {
            var gameLoop = Context.ServiceProvider.GetRequiredService<IGameLoop>();
            gameLoop.UpdateAsync();
        }

        [UsedImplicitly]
        [When(@"Следующая итерация сектора (\d+) раз")]
        public void WhenСледующаяИтерацияСектора(int count)
        {
            var gameLoop = Context.ServiceProvider.GetRequiredService<IGameLoop>();

            for (var i = 0; i < count; i++)
            {
                gameLoop.UpdateAsync();
            }
        }

        [UsedImplicitly]
        [When(@"Я выбираю ячейку \((.*), (.*)\)")]
        public void WhenЯВыбираюЯчейку(int x, int y)
        {
            Context.ClickOnNode(x, y);
        }

        [UsedImplicitly]
        [When(@"Я выбираю сундук Id:(.*)")]
        public void WhenЯВыбираюСундукId(int id)
        {
            var sectorManager = Context.ServiceProvider.GetRequiredService<ISectorManager>();
            var staticObjectManager = sectorManager.CurrentSector.StaticObjectManager;
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();

            var container = staticObjectManager.Items.Single(x => x.Id == id);

            var chestViewMdel = new TestContainerViewModel
            {
                StaticObject = container
            };

            playerState.HoverViewModel = chestViewMdel;
        }

        [UsedImplicitly]
        [When(@"Я забираю из сундука экипировку (.*)")]
        public void WhenЯЗабираюИзСундукаЭкипировкуPistol(string equipmentSchemeSid)
        {
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();
            var propTransferCommand = Context.ServiceProvider.GetRequiredService<PropTransferCommand>();

            var actor = Context.GetActiveActor();
            var container = ((IContainerViewModel)playerState.HoverViewModel).StaticObject;

            var transferMachine = new PropTransferMachine(actor.Person.GetModule<IInventoryModule>(), container.GetModule<IPropContainer>().Content);
            propTransferCommand.TransferMachine = transferMachine;

            var equipment = container.GetModule<IPropContainer>().Content.CalcActualItems().Single(x => x.Scheme.Sid == equipmentSchemeSid);

            transferMachine.TransferProp(equipment,
                PropTransferMachineStores.Container,
                PropTransferMachineStores.Inventory);

            propTransferCommand.Execute();
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

            var transferMachine = new PropTransferMachine(actor.Person.GetModule<IInventoryModule>(), container.GetModule<IPropContainer>().Content);
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
                PropTransferMachineStores.Container,
                PropTransferMachineStores.Inventory);

            propTransferCommand.Execute();
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

        [UsedImplicitly]
        [Then(@"В сундуке Id:(.*) нет экипировки (.*)")]
        public void ThenВСундукеIdНетЭкипировкиPistol(int id, string propSid)
        {
            var sectorManager = Context.ServiceProvider.GetRequiredService<ISectorManager>();
            var containerManager = sectorManager.CurrentSector.StaticObjectManager;

            var container = containerManager.Items.Single(x => x.Id == id);
            var prop = container.GetModule<IPropContainer>().Content.CalcActualItems().SingleOrDefault(x => x.Scheme.Sid == propSid);

            prop.Should().BeNull();
        }

        [UsedImplicitly]
        [Then(@"В сундуке Id:(.*) нет предмета (.*)")]
        public void ThenВСундукеIdНетПредметаWater(int containerId, string resourceSid)
        {
            var sectorManager = Context.ServiceProvider.GetRequiredService<ISectorManager>();
            var containerManager = sectorManager.CurrentSector.StaticObjectManager;

            var container = containerManager.Items.Single(x => x.Id == containerId);
            var prop = container.GetModule<IPropContainer>().Content.CalcActualItems().SingleOrDefault(x => x.Scheme.Sid == resourceSid);

            prop.Should().BeNull();
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
        [Then(@"Предмет (.*) отсутствует в инвентаре актёра")]
        public void ThenЕдаСырОтсутствуетВИнвентареПерсонажа(string propSid)
        {
            var actor = Context.GetActiveActor();

            var propsInInventory = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
            var testedProp = propsInInventory.FirstOrDefault(x => x.Scheme.Sid == propSid);

            testedProp.Should().BeNull();
        }

        [UsedImplicitly]
        [Then(@"Актёр игрока имеет запас hp (.*)")]
        public void ThenАктёрИмеетЗадасHp(int expectedHp)
        {
            var actor = Context.GetActiveActor();
            var hpStat = actor.Person.GetModule<ISurvivalModule>().Stats.Single(x => x.Type == SurvivalStatType.Health);
            hpStat.Value.Should().Be(expectedHp);
        }

        [UsedImplicitly]
        [Then(@"Монстр Id:(\d*) имеет Hp (\d*)")]
        public void ThenМонстрIdИмеетHp(int monsterId, int expectedMonsterHp)
        {
            var monster = Context.GetMonsterById(monsterId);
            var hpStat = monster.Person.GetModule<ISurvivalModule>().Stats.Single(x => x.Type == SurvivalStatType.Health);
            hpStat.Value.Should().Be(expectedMonsterHp);
        }

        [Then(@"Параметр (.*) равен (.*)")]
        public void ThenПараметр_Равен(string paramType, string paramValue)
        {
            // пока нет предметов, которые изменяют характеристики, этот метод не реализуем.
            // оставляем, чтобы после остались проверки.
        }
    }
}
