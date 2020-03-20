using UnityEngine;

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public class GenFollowingCamera : MonoBehaviour
{
    public PersonFollower PersonFollower;

    // ReSharper disable once UnusedMember.Local
    private void Update()
    {
        if (PersonFollower.FollowedPerson == null)
        {
            return;
        }

        var actorViewModel = PersonFollower.FollowedPerson;

        var targetPosition = actorViewModel.transform.position;
        var cameraPlanePosition = new Vector3(targetPosition.x, targetPosition.y, -10);

        transform.position = Vector3.Lerp(transform.position,
            cameraPlanePosition,
            Time.deltaTime * 3);
    }
}