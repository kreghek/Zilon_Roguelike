using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.PersonGeneration;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Specs.Mocks;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Contexts
{
    public abstract class FeatureContextBase
    {
        public IServiceProvider ServiceProvider { get; }

        public List<IActorInteractionEvent> RaisedActorInteractionEvents { get; }
        public RegisterServices RegisterServices { get; private set; }
        public IGlobe Globe { get; private set; }

        protected FeatureContextBase()
        {
            RaisedActorInteractionEvents = new List<IActorInteractionEvent>();

            RegisterServices = new RegisterServices();
            var serviceProvider = RegisterServices.Register();

            ServiceProvider = serviceProvider;

            Configure(serviceProvider);
        }

        private void Configure(IServiceProvider serviceProvider)
        {
            ConfigureEventBus(serviceProvider);

            RegisterManager.ConfigureAuxServices(serviceProvider);
        }

        private void ConfigureEventBus(IServiceProvider serviceProvider)
        {
            var eventMessageBus = serviceProvider.GetRequiredService<IActorInteractionBus>();
            eventMessageBus.NewEvent += EventMessageBus_NewEvent;
        }

        private void EventMessageBus_NewEvent(object sender, NewActorInteractionEventArgs e)
        {
            RaisedActorInteractionEvents.Add(e.ActorInteractionEvent);
        }

        public async Task CreateGlobeAsync(int startMapSize)
        {
            var mapFactory = (FuncMapFactory)ServiceProvider.GetRequiredService<IMapFactory>();
            mapFactory.SetFunc(() =>
            {
                ISectorMap map = new SectorGraphMap<HexNode, HexMapNodeDistanceCalculator>();

                MapFiller.FillSquareMap(map, startMapSize);

                var mapRegion = new MapRegion(1, map.Nodes.ToArray())
                {
                    IsStart = true,
                    IsOut = true,
                    ExitNodes = new[] { map.Nodes.Last() }
                };

                map.Regions.Add(mapRegion);

                return Task.FromResult(map);
            });

            var globeInitialzer = ServiceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitialzer.CreateGlobeAsync("intro").ConfigureAwait(false);
            Globe = globe;
        }

        public ISector GetSector()
        {
            var player = ServiceProvider.GetRequiredService<IPlayer>();
            return player.SectorNode.Sector;
        }

        public void AddWall(int x1, int y1, int x2, int y2)
        {
            var player = ServiceProvider.GetRequiredService<IPlayer>();

            var map = player.SectorNode.Sector.Map;

            map.RemoveEdge(x1, y1, x2, y2);
        }

        public IActor GetActiveActor()
        {
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();
            var actor = playerState.ActiveActor.Actor;
            return actor;
        }

        public Equipment CreateEquipment(string propSid)
        {
            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();
            var propFactory = ServiceProvider.GetRequiredService<IPropFactory>();

            var propScheme = schemeService.GetScheme<IPropScheme>(propSid);

            var equipment = propFactory.CreateEquipment(propScheme);
            return equipment;
        }

        public void AddHumanActor(string personSid, OffsetCoords startCoords)
        {
            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = humanPlayer.SectorNode.Sector;

            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();
            var humanTaskSource = ServiceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var actorManager = sector.ActorManager;
            var perkResolver = ServiceProvider.GetRequiredService<IPerkResolver>();

            // Подготовка актёров
            var humanStartNode = sector.Map.Nodes.SelectByHexCoords(startCoords.X, startCoords.Y);
            var humanActor = CreateHumanActor(personSid, humanStartNode, perkResolver);
            humanPlayer.BindPerson(Globe, humanActor.Person);

            actorManager.Add(humanActor);

            var humanActroViewModelMock = new Mock<IActorViewModel>();
            humanActroViewModelMock.SetupGet(x => x.Actor).Returns(humanActor);
            var humanActroViewModel = humanActroViewModelMock.Object;
            playerState.ActiveActor = humanActroViewModel;
        }

        public void AddMonsterActor(string monsterSid, int monsterId, OffsetCoords startCoords)
        {
            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = humanPlayer.SectorNode.Sector;

            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();
            var actorManager = sector.ActorManager;

            var monsterScheme = schemeService.GetScheme<IMonsterScheme>(monsterSid);
            var monsterStartNode = sector.Map.Nodes.SelectByHexCoords(startCoords.X, startCoords.Y);

            var monster = CreateMonsterActor(monsterScheme, monsterStartNode);
            monster.Person.Id = monsterId;

            actorManager.Add(monster);
        }

        public IPropContainer AddChest(int id, OffsetCoords nodeCoords)
        {
            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = humanPlayer.SectorNode.Sector;

            var node = sector.Map.Nodes.SelectByHexCoords(nodeCoords.X, nodeCoords.Y);
            var chest = new FixedPropChest(Array.Empty<IProp>());
            var staticObject = new StaticObject(node, chest.Purpose, id);
            staticObject.AddModule<IPropContainer>(chest);

            sector.StaticObjectManager.Add(staticObject);

            return chest;
        }

        public void AddResourceToActor(string resourceSid, int count, IActor actor)
        {
            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();

            var resourceScheme = schemeService.GetScheme<IPropScheme>(resourceSid);

            AddResourceToActor(resourceScheme, count, actor);
        }

        public static void AddResourceToActor(IPropScheme resourceScheme, int count, IActor actor)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var resource = new Resource(resourceScheme, count);

            actor.Person.GetModule<IInventoryModule>().Add(resource);
        }

        public IActor GetMonsterById(int id)
        {
            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = humanPlayer.SectorNode.Sector;

            var actorManager = sector.ActorManager;

            var monster = actorManager.Items
                .SingleOrDefault(x => x.Person is MonsterPerson && x.Person.Id == id);

            return monster;
        }

        private IActor CreateHumanActor([NotNull] string personSchemeSid,
            [NotNull] IGraphNode startNode,
            [NotNull] IPerkResolver perkResolver)
        {
            var personFactory = ServiceProvider.GetRequiredService<IPersonFactory>();
            var humanTaskSource = ServiceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();

            var person = personFactory.Create(personSchemeSid, Fractions.MainPersonFraction);

            var actor = new Actor(person, humanTaskSource, startNode, perkResolver);

            return actor;
        }

        private IActor CreateMonsterActor([NotNull] IMonsterScheme monsterScheme,
            [NotNull] IGraphNode startNode)
        {
            var monsterFactory = ServiceProvider.GetRequiredService<IMonsterPersonFactory>();

            var taskSource = ServiceProvider.GetRequiredService<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();

            var monsterPerson = monsterFactory.Create(monsterScheme);

            var actor = new Actor(monsterPerson, taskSource, startNode);

            return actor;
        }

        public class VisualEventInfo
        {
            public VisualEventInfo(IActor actor, EventArgs eventArgs, string eventName)
            {
                Actor = actor ?? throw new ArgumentNullException(nameof(actor));
                EventArgs = eventArgs ?? throw new ArgumentNullException(nameof(eventArgs));
                EventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
            }

            public IActor Actor { get; }

            public EventArgs EventArgs { get; }

            public string EventName { get; }
        }
    }
}