using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatMapVM : MonoBehaviour
{

    public CombatLocationVM LocationPrefab;
    public CombatPathVM PathPrefab;
    public CombatActorVM ActorPrefab;
    public Canvas Canvas;

    private List<CombatLocationVM> _locations;
    private List<CombatLocationVM> _teamLocations;

    public object x { get; private set; }

    private void Awake()
    {
        CreateLocations();
        CreateActors();
    }

    private void CreateLocations()
    {
        _locations = new List<CombatLocationVM>();
        _teamLocations = new List<CombatLocationVM>();
        const int mapSize = 10;
        for (var i = 0; i < mapSize; i++)
        {
            for (var j = 0; j < mapSize; j++)
            {
                var location = Instantiate(LocationPrefab, transform);
                var offset = UnityEngine.Random.insideUnitCircle * 10;
                location.transform.position = new Vector3(20 * i + offset.x, 20 * j + offset.y);

                _locations.Add(location);

                if (i == 0 || i == mapSize-1)
                {
                    if (j == 0 || j == mapSize-1)
                    {
                        _teamLocations.Add(location);
                    }
                }
            }
        }
    }

    private void CreateActors()
    {
        for (var teamIndex = 0; teamIndex < 2; teamIndex++)
        {
            var teamLocation = _teamLocations[teamIndex];
            var quadLocations = _locations.Where(x => (x.transform.position - teamLocation.transform.position).magnitude < 40).ToArray();
            for (var quadIndex = 0; quadIndex < 3; quadIndex++)
            {
                var quadLocation = quadLocations[UnityEngine.Random.Range(0, quadLocations.Length)];

                for (var actorIndex = 0; actorIndex < 5 + quadIndex; actorIndex++)
                {
                    var actor = Instantiate(ActorPrefab, transform);
                    var offset = UnityEngine.Random.insideUnitCircle * 2;
                    actor.transform.position = quadLocation.transform.position + new Vector3(offset.x, offset.y);
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
