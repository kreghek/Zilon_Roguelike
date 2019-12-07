using System.Linq;

using UnityEngine;

public class PersonFollower : MonoBehaviour
{
    public ActorsViewModel ActorsViewModel;

    public ActorViewModel FollowedPerson { get; private set; }

    public bool IsFollowing { get => FollowedPerson != null; }

    public void Update()
    {
        if (FollowedPerson != null)
        {
            return;
        }

        TryFollowFirstPersonInSector();
    }

    private void TryFollowFirstPersonInSector()
    {
        if (!ActorsViewModel.ActorViewModels.Any())
        {
            return;
        }

        var actorViewModel = ActorsViewModel.ActorViewModels.First();
        FollowedPerson = actorViewModel;
    }
}
