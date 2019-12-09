using System.Linq;

using UnityEngine;

using Zilon.Core.Persons;

public class PersonFollower : MonoBehaviour
{
    /// <summary>
    /// Время, в течении которого нужно следить за одним персонажем.
    /// В секундах.
    /// </summary>
    private const float FOLLOW_DURATION_SEC = 2 * 60;
    private float _followCounter;

    public ActorsViewModel ActorsViewModel;

    public ActorViewModel FollowedPerson { get; private set; }

    public bool IsFollowing { get => FollowedPerson != null; }

    public void FixedUpdate()
    {
        if (IsFollowing)
        {
            if (FollowedPerson.Actor.Person.CheckIsDead())
            {
                // Наблюдаемый объект мертв.
                TrySelectPersonInSector();
            }
            else
            {
                // Наблюдение ведётся.
                // Либо выжидаем лимит времени наблюдения.
                // Либо меняем объект наблюдения.
                UpdateFollowing();
            }
        }
        else
        {
            // В данный момент никто не наблюдается.
            TrySelectPersonInSector();
        }
    }

    private void UpdateFollowing()
    {
        _followCounter -= Time.deltaTime;

        if (_followCounter <= 0)
        {
            _followCounter = FOLLOW_DURATION_SEC;

            TrySelectPersonInSector();
        }
    }

    private void TrySelectPersonInSector()
    {
        var aliveActors = ActorsViewModel.ActorViewModels.Where(x => !x.Actor.Person.CheckIsDead()).ToArray();

        if (!aliveActors.Any())
        {
            return;
        }

        var actorViewModel = SelectRandomActor(aliveActors);

        FollowActorInSector(actorViewModel);
    }

    private static ActorViewModel SelectRandomActor(ActorViewModel[] aliveActors)
    {
        var personCount = aliveActors.Count();

        var randomPersonIndex = Random.Range(0, personCount);

        var actorViewModel = aliveActors[randomPersonIndex];
        return actorViewModel;
    }

    private void FollowActorInSector(ActorViewModel actorViewModel)
    {
        FollowedPerson = actorViewModel;
    }
}
