using UnityEngine;

public class LoadingIndicatorHandler : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.Rotate(Vector3.forward, Time.deltaTime * 100f);
    }
}
