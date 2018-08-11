using System.Configuration;
using System.Linq;

using FluentAssertions;

using JetBrains.Annotations;

using LightInject;

using Moq;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Spec
{
    [Binding]
    public class Bug_EquipmentAfterContainerLoot
    {
        private ServiceContainer _container;
        private IContainerViewModel _targetChest;
        private PropTransferMachine _containerModalTransferMachine;

        [Given(@"Персонаж стоит возле сундука с ресурсами")]
        public void GivenПерсонажСтоитВозлеСундукаСРесурсами()
        {
            _container = new ServiceContainer();

            RegisterSchemeService();
            RegisterMap();
            RegisterAuxServices();
            RegisterFakeClientServices();

            _container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            _container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
            _container.Register<ISector, Sector>(new PerContainerLifetime());

            _container.Register<ICommand, OpenContainerCommand>("open-container-command", new PerContainerLifetime());
            _container.Register<ICommand, ShowContainerModalCommand>("show-container-modal-command", new PerContainerLifetime());
            //TODO Сделать регистрацию команды в контейнере аналогично EquipCommand
            //_container.Register<ICommand, PropTransferCommand>("prop-transfer-command", new PerContainerLifetime());
            _container.Register<ICommand, ShowInventoryModalCommand>("show-inventory-modal-command", new PerContainerLifetime());
            _container.Register<ICommand, EquipCommand>("equip-command");

            GenerateSectorTtc4Content();
        }
        
        [When(@"Я открываю сундук")]
        public void WhenЯОткрываюСундук()
        {
            var playerState = _container.GetInstance<IPlayerState>();
            var openContainerCommand = _container.GetInstance<ICommand>("open-container-command");

            playerState.HoverViewModel = _targetChest;
            openContainerCommand.Execute();
        }

        [When(@"Я открываю окно с содержимым сундука")]
        public void WhenЯОткрываюОкноССодержимымСундука()
        {
            var showContainerModalCommand = _container.GetInstance<ICommand>("show-container-modal-command");
            //var playerState = _container.GetInstance<IPlayerState>();
            //var modelManager = _container.GetInstance<ISectorModalManager>();

            //var actor = playerState.ActiveActor.Actor;
            //var chest = ((IContainerViewModel)playerState.HoverViewModel).Container;
            //var transferMachine = new PropTransferMachine(actor.Person.Inventory, chest.Content);

            //modelManager.ShowContainerModal(transferMachine);

            showContainerModalCommand.Execute();
        }

        [When(@"Я переношу содержимое себе в инвентарь")]
        public void WhenЯПереношуСодержимоеСебеВИнвентарь()
        {
            var playerState = _container.GetInstance<IPlayerState>();
            var sectorManager = _container.GetInstance<ISectorManager>();
            //TODO Сделать регистрацию команды в контейнере аналогично EquipCommand
            //var propTransferCommand = _container.GetInstance<ICommand>("prop-transfer-command");
            
            
            //TODO Сделать регистрацию команды в контейнере аналогично EquipCommand
            var propTransferCommand = new PropTransferCommand(sectorManager, playerState, _containerModalTransferMachine);

            var props = _containerModalTransferMachine.Container.CalcActualItems();
            foreach (var prop in props)
            {
                _containerModalTransferMachine.TransferProp(prop,
                    _containerModalTransferMachine.Container,
                    _containerModalTransferMachine.Inventory);
            }

            propTransferCommand.Execute();
        }
        
        [When(@"Я открываю окна инвентаря")]
        public void WhenЯОткрываюОкнаИнвентаря()
        {
            var showInventoryModalCommand = _container.GetInstance<ICommand>("show-inventory-modal-command");
            showInventoryModalCommand.Execute();
        }
        
        [When(@"Я назначаю экипировку из инвентаря")]
        public void WhenЯНазначаюЭкипировкуИзИнвентаря()
        {
            var equipCommand = (EquipCommand)_container.GetInstance<ICommand>("equip-command");
            var inventoryState = _container.GetInstance<IInventoryState>();
            var playerState = _container.GetInstance<IPlayerState>();

            equipCommand.SlotIndex = 0;

            // Эмулируем выбор экипировки в инвентаре.
            var actor = playerState.ActiveActor.Actor;
            var pistolEquipment = actor.Person.Inventory.CalcActualItems().SingleOrDefault(x => x.Scheme.Sid == "pistol");
            var selectedEquipmentViewModelMock = new Mock<IPropItemViewModel>();
            selectedEquipmentViewModelMock.SetupGet(x => x.Prop).Returns(pistolEquipment);
            var selectedEquipmentViewModel = selectedEquipmentViewModelMock.Object;
            inventoryState.SelectedProp = selectedEquipmentViewModel;

            equipCommand.Execute();
        }
        
        [Then(@"Персонажу назначена экипировка")]
        public void ThenПерсонажуНазначенаЭкипировка()
        {
            var playerState = _container.GetInstance<IPlayerState>();

            var actor = playerState.ActiveActor.Actor;
            var currentEquipment = actor.Person.EquipmentCarrier.Equipments[0];
            currentEquipment.Scheme.Sid.Should().Be("pistol");
        }

        [Then(@"Текущая экипировка перенесена в инвентарь")]
        public void ThenТекущаяЭкипировкаПеренесенаВИнвентарь()
        {
            var playerState = _container.GetInstance<IPlayerState>();

            var actor = playerState.ActiveActor.Actor;
            var oldEquipment = actor.Person.Inventory.CalcActualItems().SingleOrDefault(x=>x.Scheme.Sid == "short-sword");
            oldEquipment.Should().NotBeNull();
        }


        private void RegisterMap()
        {
            _container.Register<IMap>(factory =>
            {
                var map = new TestGridGenMap(2);
                return map;
            });
        }

        private void RegisterSchemeService()
        {
            _container.Register<ISchemeLocator>(factory =>
            {
                var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            }, new PerContainerLifetime());

            _container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());

            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices()
        {
            _container.Register<IDice>(factory => new Dice(), new PerContainerLifetime());
            _container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            _container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            _container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            _container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
        }

        private void RegisterFakeClientServices()
        {
            _container.Register<IPlayerState, PlayerState>(new PerContainerLifetime());
            _container.Register<ISectorManager, SectorManager>(new PerContainerLifetime());
            _container.Register<IInventoryState, InventoryState>(new PerContainerLifetime());

            var modalManagerMock = new Mock<ISectorModalManager>();
            modalManagerMock.Setup(x => x.ShowContainerModal(It.IsAny<PropTransferMachine>()))
                .Callback<PropTransferMachine>(transferMachine => {
                    _containerModalTransferMachine = transferMachine;
                });
            var modalManager = modalManagerMock.Object;
            _container.Register(factory => modalManager, new PerContainerLifetime());
        }

        private void GenerateSectorTtc4Content()
        {
            var schemeService = _container.GetInstance<ISchemeService>();
            var map = _container.GetInstance<IMap>();
            var sector = _container.GetInstance<ISector>();
            var sectorManager = _container.GetInstance<ISectorManager>();
            var playerState = _container.GetInstance<IPlayerState>();


            sectorManager.CurrentSector = sector;


            // Подготовка игроков
            var humanPlayer = new HumanPlayer();

            var personScheme = schemeService.GetScheme<PersonScheme>("captain");

            // Подготовка актёров
            var humanStartNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);
            var humanActor = CreateHumanActor(humanPlayer, personScheme, humanStartNode);
            var humanActroViewModelMock = new Mock<IActorViewModel>();
            humanActroViewModelMock.SetupGet(x => x.Actor).Returns(humanActor);
            var humanActroViewModel = humanActroViewModelMock.Object;
            playerState.ActiveActor = humanActroViewModel;

            // Подготовка сундуков с предметами
            var chestNode = map.Nodes.Cast<HexNode>().SelectBy(1, 0);
            var loot = CreateChestLoot();
            var chest = new FixedPropContainer(chestNode, loot);

            var chestViewModelMock = new Mock<IContainerViewModel>();
            chestViewModelMock.SetupGet(x => x.Container).Returns(chest);
            _targetChest = chestViewModelMock.Object;



            // Подготовка источника поведения ботов
            var decisionSource = _container.GetInstance<IDecisionSource>();
            var tacticalActUsageService = _container.GetInstance<ITacticalActUsageService>();

            var humanTaskSource = new HumanActorTaskSource(decisionSource, tacticalActUsageService);
            humanTaskSource.SwitchActor(humanActor);
            playerState.TaskSource = humanTaskSource;

            _container.Register(factory => humanTaskSource);


            ((Sector)sector).BehaviourSources = new IActorTaskSource[]
            {
                humanTaskSource
            };
        }

        private IActor CreateHumanActor([NotNull] IPlayer player,
            [NotNull] PersonScheme personScheme,
            [NotNull] IMapNode startNode)
        {
            var actorManager = _container.GetInstance<IActorManager>();
            var schemeService = _container.GetInstance<ISchemeService>();

            var evolutionData = new EvolutionData(schemeService);

            var inventory = new Inventory();

            var person = new HumanPerson(personScheme, evolutionData, inventory);

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            // Указываем экипировку по умолчанию.
            var equipment = CreateEquipment("short-sword");
            actor.Person.EquipmentCarrier.SetEquipment(equipment, 0);

            // Второе оружие в инвернтаре
            var pistolEquipment = CreateEquipment("pistol");
            inventory.Add(pistolEquipment);

            return actor;
        }

        private Equipment CreateEquipment(string propSid)
        {
            var schemeService = _container.GetInstance<ISchemeService>();
            var propFactory = _container.GetInstance<IPropFactory>();

            var propScheme = schemeService.GetScheme<PropScheme>(propSid);
            var equipment = propFactory.CreateEquipment(propScheme);
            return equipment;
        }

        private IProp[] CreateChestLoot()
        {
            var schemeService = _container.GetInstance<ISchemeService>();

            var propScheme = schemeService.GetScheme<PropScheme>("packed-food");

            var resource = new Resource(propScheme, 1);

            return new IProp[] { resource };
        }
    }
}
