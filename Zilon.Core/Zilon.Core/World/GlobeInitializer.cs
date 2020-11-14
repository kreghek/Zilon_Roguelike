using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    public sealed class GlobeInitializer : IGlobeInitializer
    {
        private readonly IActorTaskSource<ISectorTaskSourceContext> _actorTaskSource;
        private readonly IBiomeInitializer _biomeInitializer;
        private readonly IGlobeTransitionHandler _globeTransitionHandler;
        private readonly IPersonInitializer _personInitializer;
        private readonly ISchemeService _schemeService;

        public GlobeInitializer(
            IBiomeInitializer biomeInitializer,
            IGlobeTransitionHandler globeTransitionHandler,
            ISchemeService schemeService,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource,
            IPersonInitializer personInitializer)
        {
            _biomeInitializer = biomeInitializer;
            _globeTransitionHandler = globeTransitionHandler;
            _schemeService = schemeService;
            _actorTaskSource = actorTaskSource;
            _personInitializer = personInitializer;
        }

        public async Task<IGlobe> CreateGlobeAsync(string startLocationSchemeSid)
        {
            var globe = new Globe(_globeTransitionHandler);

            var startLocation = _schemeService.GetScheme<ILocationScheme>(startLocationSchemeSid);
            var startBiom = await _biomeInitializer.InitBiomeAsync(startLocation).ConfigureAwait(false);
            var startSectorNode = startBiom.Sectors.First(x => x.State == SectorNodeState.SectorMaterialized);

            globe.AddSectorNode(startSectorNode);

            // Добавляем стартовых персонажей-пилигримов

            var startPersons = await _personInitializer.CreateStartPersonsAsync(globe).ConfigureAwait(false);

            var sector = startSectorNode.Sector;
            var personCounter = 0;
            foreach (var person in startPersons)
            {
                var startNode = sector.Map
                    .Nodes
                    .Skip(personCounter)
                    .First();
                var actor = CreateActor(person, startNode, _actorTaskSource);

                sector.ActorManager.Add(actor);
                personCounter++;
            }

            return globe;
        }

        private static IActor CreateActor(
            IPerson humanPerson,
            IGraphNode startNode,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource)
        {
            var actor = new Actor(humanPerson, actorTaskSource, startNode);

            return actor;
        }
    }
}