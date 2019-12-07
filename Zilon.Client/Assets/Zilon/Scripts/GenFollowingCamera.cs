using System.Linq;

using UnityEngine;

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public class GenFollowingCamera : MonoBehaviour
{
    public ActorsViewModel ActorsViewModel;

    // ReSharper disable once UnusedMember.Local
    private void Update()
    {
        if (!ActorsViewModel.ActorViewModels.Any())
        {
            return;
        }

        var actorViewModel = ActorsViewModel.ActorViewModels.First();

        var targetPosition = actorViewModel.transform.position;
        var cameraPlanePosition = new Vector3(targetPosition.x, targetPosition.y, -10);

        transform.position = Vector3.Lerp(transform.position,
            cameraPlanePosition,
            Time.deltaTime * 3);
    }
}