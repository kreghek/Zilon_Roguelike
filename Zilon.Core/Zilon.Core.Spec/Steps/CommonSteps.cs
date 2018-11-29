using System.Linq;

using FluentAssertions;

using JetBrains.Annotations;

using LightInject;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Spec.Steps
{
    [UsedImplicitly]
    [Binding]
    public class CommonSteps: GenericStepsBase<CommonGameActionsContext>
    {
        [UsedImplicitly]
        public CommonSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [UsedImplicitly]
        [Given(@"Есть карта размером (.*)")]
        public void GivenЕстьКартаРазмером(int mapSize)
        {
            Context.CreateSector(mapSize);
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
            actor.Person.Survival.SetStatForce(SurvivalStatType.Health, startHp);
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

            monster.Person.Survival.SetStatForce(SurvivalStatType.Health, monsterHp);
        }

        [UsedImplicitly]
        [Given(@"Есть сундук Id:(.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьСундукВЯчейке(int id,  int offsetX, int offsetY)
        {
            var coords = new OffsetCoords(offsetX, offsetY);
            Context.AddChest(id, coords);
        }

        [UsedImplicitly]
        [Given(@"Сундук содержит Id:(.*) экипировку (.*)")]
        public void GivenСундукСодержитIdЭкипировкуPistol(int id, string equipmentSid)
        {
            var containerManager = Context.Container.GetInstance<IPropContainerManager>();
            var propFactory = Context.Container.GetInstance<IPropFactory>();
            var schemeService = Context.Container.GetInstance<ISchemeService>();

            var container = containerManager.Items.Single(x => x.Id == id);

            var propScheme = schemeService.GetScheme<IPropScheme>(equipmentSid);
            var equipment = propFactory.CreateEquipment(propScheme);

            container.Content.Add(equipment);
        }

        [UsedImplicitly]
        [Given(@"Сундук содержит Id:(.*) ресурс (.*) в количестве (.*)")]
        public void GivenСундукСодержитIdРусурсPistol(int id, string resourceSid, int count)
        {
            var containerManager = Context.Container.GetInstance<IPropContainerManager>();
            var propFactory = Context.Container.GetInstance<IPropFactory>();
            var schemeService = Context.Container.GetInstance<ISchemeService>();

            var container = containerManager.Items.Single(x => x.Id == id);

            var propScheme = schemeService.GetScheme<IPropScheme>(resourceSid);
            var resource = propFactory.CreateResource(propScheme, count);

            container.Content.Add(resource);
        }

        [UsedImplicitly]
        [When(@"Следующая итерация сектора")]
        public void WhenСледующаяИтерацияСектора()
        {
            //TODO Заменить на выполнение специальной команды смены итерации (её ещё нет)
            var gameLoop = Context.Container.GetInstance<IGameLoop>();
            gameLoop.Update();
        }

        [UsedImplicitly]
        [When(@"Я выбираю ячейку \((.*), (.*)\)")]
        public void WhenЯВыбираюЯчейку(int x, int y)
        {
            Context.HoverNode(x, y);
        }

        [UsedImplicitly]
        [When(@"Я выбираю сундук Id:(.*)")]
        public void WhenЯВыбираюСундукId(int id)
        {
            var containerManager = Context.Container.GetInstance<IPropContainerManager>();
            var playerState = Context.Container.GetInstance<IPlayerState>();

            var container = containerManager.Items.Single(x => x.Id == id);

            var chestViewMdel = new TestContainerViewModel
            {
                Container = container
            };

            playerState.HoverViewModel = chestViewMdel;
        }

        [UsedImplicitly]
        [When(@"Я забираю из сундука экипировку (.*)")]
        public void WhenЯЗабираюИзСундукаЭкипировкуPistol(string equipmentSchemeSid)
        {
            var playerState = Context.Container.GetInstance<IPlayerState>();
            var propTransferCommand = Context.Container.GetInstance<ICommand>("prop-transfer");

            var actor = Context.GetActiveActor();
            var container = ((IContainerViewModel)playerState.HoverViewModel).Container;

            var transferMachine = new PropTransferMachine(actor.Person.Inventory, container.Content);
            ((PropTransferCommand)propTransferCommand).TransferMachine = transferMachine;

            var equipment = container.Content.CalcActualItems().Single(x=>x.Scheme.Sid == equipmentSchemeSid);

            transferMachine.TransferProp(equipment, container.Content, actor.Person.Inventory);

            propTransferCommand.Execute();
        }

        [UsedImplicitly]
        [When(@"Я забираю из сундука рерурс (.*) в количестве (.*)")]
        public void WhenЯЗабираюИзСундукаРерурсWaterВКоличестве(string resourceSid, int count)
        {
            var propFactory = Context.Container.GetInstance<IPropFactory>();
            var playerState = Context.Container.GetInstance<IPlayerState>();
            var propTransferCommand = Context.Container.GetInstance<ICommand>("prop-transfer");

            var actor = Context.GetActiveActor();
            var container = ((IContainerViewModel)playerState.HoverViewModel).Container;

            var transferMachine = new PropTransferMachine(actor.Person.Inventory, container.Content);
            ((PropTransferCommand)propTransferCommand).TransferMachine = transferMachine;

            var resource = container.Content.CalcActualItems()
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
            

            transferMachine.TransferProp(takenResource, container.Content, actor.Person.Inventory);

            propTransferCommand.Execute();
        }


        [UsedImplicitly]
        [Then(@"У актёра в инвентаре есть (.*)")]
        public void ThenУАктёраВИнвентареЕстьPistol(string equipmentSchemeSid)
        {
            var actor = Context.GetActiveActor();

            var inventoryItems = actor.Person.Inventory.CalcActualItems();
            var foundEquipment = inventoryItems.SingleOrDefault(x=>x.Scheme.Sid == equipmentSchemeSid);

            foundEquipment.Should().NotBeNull();
        }

        [UsedImplicitly]
        [Then(@"В сундуке Id:(.*) нет экипировки (.*)")]
        public void ThenВСундукеIdНетЭкипировкиPistol(int id, string propSid)
        {
            var containerManager = Context.Container.GetInstance<IPropContainerManager>();

            var container = containerManager.Items.Single(x => x.Id == id);
            var prop = container.Content.CalcActualItems().SingleOrDefault(x => x.Scheme.Sid == propSid);

            prop.Should().BeNull();
        }

        [UsedImplicitly]
        [Then(@"В сундуке Id:(.*) нет предмета (.*)")]
        public void ThenВСундукеIdНетПредметаWater(int containerId, string resourceSid)
        {
            var containerManager = Context.Container.GetInstance<IPropContainerManager>();

            var container = containerManager.Items.Single(x => x.Id == containerId);
            var prop = container.Content.CalcActualItems().SingleOrDefault(x => x.Scheme.Sid == resourceSid);

            prop.Should().BeNull();
        }

        [UsedImplicitly]
        [Then(@"Предмет (.*) отсутствует в инвентаре актёра")]
        public void ThenЕдаСырОтсутствуетВИнвентареПерсонажа(string propSid)
        {
            var actor = Context.GetActiveActor();

            var propsInInventory = actor.Person.Inventory.CalcActualItems();
            var testedProp = propsInInventory.FirstOrDefault(x => x.Scheme.Sid == propSid);

            testedProp.Should().BeNull();
        }

        [UsedImplicitly]
        [Then(@"Актёр игрока имеет запас hp (.*)")]
        public void ThenАктёрИмеетЗадасHp(int expectedHp)
        {
            var actor = Context.GetActiveActor();
            var hpStat = actor.Person.Survival.Stats.Single(x => x.Type == SurvivalStatType.Health);
            hpStat.Value.Should().Be(expectedHp);
        }

        [UsedImplicitly]
        [Then(@"Монстр Id:(.*) имеет Hp (.*)")]
        public void ThenМонстрIdИмеетHp(int monsterId, int expectedMonsterHp)
        {
            var monster = Context.GetMonsterById(monsterId);
            var hpStat = monster.Person.Survival.Stats.Single(x => x.Type == SurvivalStatType.Health);
            hpStat.Value.Should().Be(expectedMonsterHp);
        }

    }
}
