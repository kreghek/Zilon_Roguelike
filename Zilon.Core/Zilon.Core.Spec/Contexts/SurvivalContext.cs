using System.Linq;

using JetBrains.Annotations;

using LightInject;

using Moq;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Spec.Contexts
{
    public class SurvivalContext : FeatureContextBase
    {
        public void CreateSector()
        {
            var map = new TestGridGenMap(2);

            _container.Register<IMap>(factory => map);
            _container.Register<ISector, Sector>();
        }

        public void AddHumanActor(OffsetCoords startCoords)
        {
            var playerState = _container.GetInstance<IPlayerState>();
            var schemeService = _container.GetInstance<ISchemeService>();
            var map = _container.GetInstance<IMap>();
            var humanTaskSource = _container.GetInstance<IHumanActorTaskSource>();

            var personScheme = schemeService.GetScheme<PersonScheme>("captain");

            // Подготовка актёров
            var humanStartNode = map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);
            var humanActor = CreateHumanActor(_humanPlayer, personScheme, humanStartNode);

            humanTaskSource.SwitchActor(humanActor);

            var humanActroViewModelMock = new Mock<IActorViewModel>();
            humanActroViewModelMock.SetupGet(x => x.Actor).Returns(humanActor);
            var humanActroViewModel = humanActroViewModelMock.Object;
            playerState.ActiveActor = humanActroViewModel;
        }

        public void MoveOnceActiveActor(OffsetCoords targetCoords)
        {
            var playerState = _container.GetInstance<IPlayerState>();
            var moveCommand = _container.GetInstance<ICommand>("move");
            var map = _container.GetInstance<IMap>();

            var targetNode = map.Nodes.Cast<HexNode>().SelectBy(targetCoords.X, targetCoords.Y);
            var nodeViewModel = new TestNodeViewModel();
            nodeViewModel.Node = targetNode;
            playerState.HoverViewModel = nodeViewModel;
            moveCommand.Execute();
        }

        public void AddResourceToActor(string resourceSid, int count, IActor actor)
        {
            var schemeService = _container.GetInstance<ISchemeService>();

            var resourceScheme = schemeService.GetScheme<PropScheme>(resourceSid);

            var resource = new Resource(resourceScheme, count);

            actor.Person.Inventory.Add(resource);
        }

        public void UsePropByActiveActor(string propSid)
        {
            var useSelfCommand = _container.GetInstance<ICommand>("use-self");
            var inventoryState = _container.GetInstance<IInventoryState>();
            var actor = GetActiveActor();

            var selectedProp = actor.Person.Inventory.CalcActualItems().First(x => x.Scheme.Sid == propSid);

            var viewModel = new TestPropItemViewModel()
            {
                Prop = selectedProp
            };
            inventoryState.SelectedProp = viewModel;

            useSelfCommand.Execute();
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
    }
}
