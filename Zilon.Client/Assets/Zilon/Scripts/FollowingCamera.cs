using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
// ReSharper disable once UnusedMember.Global
public class FollowingCamera : MonoBehaviour
{
    private ActorVM _target;

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        var actors = FindObjectsOfType<ActorVM>();
        _target = actors.Single(x => !x.IsEnemy);

        if (_target == null)
        {
            Debug.LogError("Не найден актёр для фокуса камеры.");
            gameObject.SetActive(false);
        }
    }


    // ReSharper disable once UnusedMember.Local
    private void Update()
    {
        if (_target == null)
        {
            Debug.LogError("Не найден актёр для фокуса камеры.");
            gameObject.SetActive(false);
            return;
        }

        transform.position = Vector3.Lerp(transform.position,
            _target.transform.position + new Vector3(0, 0, -10),
            Time.deltaTime * 3);
    }
}