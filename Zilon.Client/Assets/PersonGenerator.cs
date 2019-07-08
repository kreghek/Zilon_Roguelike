using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;

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

    [Inject] IHumanPersonFactory _humanPersonFactory;

    public PersonGenerator()
    {
        _persons = new GameObject[SPAWN_MATRIX_WIDTH * SPAWN_MATRIX_HEIGHT];
    }

    public void FixedUpdate()
    {
        _spawnCounter += Time.deltaTime * SPAWN_SPEED;
        if (_spawnCounter > 1)
        {
            _spawnCounter = 0;
            CreateNewPerson();
        }
    }

    private void CreateNewPerson()
    {
        var person = _humanPersonFactory.Create();

    }
}
