using System;

using UnityEngine;

public class GlobalFollowCamera : MonoBehaviour
{
    public GameObject Target;

    private void Update()
    {
        if (Target == null)
        {
            throw new ArgumentException("Не указан объект слежения.");
        }

        transform.position = Vector3.Lerp(transform.position,
            Target.transform.position + new Vector3(0, 0, -10),
            Time.deltaTime * 3);
    }
}
