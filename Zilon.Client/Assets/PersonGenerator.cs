using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

public class PersonGenerator : MonoBehaviour
{
    [NotNull] public ActorViewModel ActorPrefab;

    [NotNull] public GunShootTracer GunShootTracer;

    [NotNull] public HumanoidActorGraphic HumanoidGraphicPrefab;

    [NotNull] public MonoActorGraphic MonoGraphicPrefab;

    private const float SPAWN_SPEED = 1f;
    private const int SPAWN_MATRIX_WIDTH = 5;
    private const int SPAWN_MATRIX_HEIGHT = 5;

    private float _spawnCounter;
    private int _spawnIndex;
    private GameObject[] _persons;
    private readonly HumanPlayer _humanPlayer;
    private readonly HexNode _node;

    [Inject]
    private readonly IHumanPersonFactory _humanPersonFactory;

    [Inject]
    private readonly IPerkResolver _perkResolver;

    [NotNull] [Inject]
    private readonly DiContainer _container;

    public PersonGenerator()
    {
        _persons = new GameObject[SPAWN_MATRIX_WIDTH * SPAWN_MATRIX_HEIGHT];

        _humanPlayer = new HumanPlayer();

        _node = new HexNode(0, 0);
    }

    public void FixedUpdate()
    {
        _spawnCounter += Time.deltaTime * SPAWN_SPEED;
        if (_spawnCounter > 1)
        {
            _spawnCounter = 0;
            SpawnNewPerson();
        }
    }

    private void SpawnNewPerson()
    {
        var person = _humanPersonFactory.Create();

        var actorViewModel = CreateHumanActorViewModel(person, _humanPlayer, _node);

    }

    private ActorViewModel CreateHumanActorViewModel(
        [NotNull] IPerson person,
        [NotNull] IPlayer player,
        [NotNull] IMapNode startNode)
    {
        var actor = new Actor(person, player, startNode, _perkResolver);

        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();
        var actorGraphic = Instantiate(HumanoidGraphicPrefab, actorViewModel.transform);
        actorGraphic.transform.position = new Vector3(0, 0.2f, 0);
        actorViewModel.GraphicRoot = actorGraphic;

        var graphicController = actorViewModel.gameObject.AddComponent<HumanActorGraphicController>();
        graphicController.Actor = actor;
        graphicController.Graphic = actorGraphic;

        return actorViewModel;
    }
}
