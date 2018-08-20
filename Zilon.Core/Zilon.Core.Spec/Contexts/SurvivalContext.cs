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

namespace Zilon.Core.Spec.Contexts
{
    public class SurvivalContext : FeatureContextBase
    {
        public void CreateSector(int mapSize)
        {
            var map = new TestGridGenMap(mapSize);

            Container.Register<IMap>(factory => map);
            Container.Register<ISector, Sector>();
        }

        public void AddHumanActor(string personSid, OffsetCoords startCoords)
        {
            var playerState = Container.GetInstance<IPlayerState>();
            var schemeService = Container.GetInstance<ISchemeService>();
            var map = Container.GetInstance<IMap>();
            var humanTaskSource = Container.GetInstance<IHumanActorTaskSource>();

            var personScheme = schemeService.GetScheme<PersonScheme>(personSid);

            // Подготовка актёров
            var humanStartNode = map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);
            var humanActor = CreateHumanActor(HumanPlayer, personScheme, humanStartNode);

            humanTaskSource.SwitchActor(humanActor);

            var humanActroViewModelMock = new Mock<IActorViewModel>();
            humanActroViewModelMock.SetupGet(x => x.Actor).Returns(humanActor);
            var humanActroViewModel = humanActroViewModelMock.Object;
            playerState.ActiveActor = humanActroViewModel;
        }

        public void MoveOnceActiveActor(OffsetCoords targetCoords)
        {
            var playerState = Container.GetInstance<IPlayerState>();
            var moveCommand = Container.GetInstance<ICommand>("move");
            var map = Container.GetInstance<IMap>();

            var targetNode = map.Nodes.Cast<HexNode>().SelectBy(targetCoords.X, targetCoords.Y);
            var nodeViewModel = new TestNodeViewModel
            {
                Node = targetNode
            };

            playerState.HoverViewModel = nodeViewModel;

            moveCommand.Execute();
        }

        public void AddResourceToActor(string resourceSid, int count, IActor actor)
        {
            var schemeService = Container.GetInstance<ISchemeService>();

            var resourceScheme = schemeService.GetScheme<PropScheme>(resourceSid);

            var resource = new Resource(resourceScheme, count);

            actor.Person.Inventory.Add(resource);
        }

        public void UsePropByActiveActor(string propSid)
        {
            var useSelfCommand = Container.GetInstance<ICommand>("use-self");
            var inventoryState = Container.GetInstance<IInventoryState>();
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
            var actorManager = Container.GetInstance<IActorManager>();
            var schemeService = Container.GetInstance<ISchemeService>();

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
    }
}
