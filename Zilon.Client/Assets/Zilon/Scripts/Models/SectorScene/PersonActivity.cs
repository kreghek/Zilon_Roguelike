using UnityEngine;

public class PersonActivity : MonoBehaviour
{
    private float _rotationCounter;

    private void Update()
    {
        _rotationCounter += Time.deltaTime * 3;
        var angle = Mathf.Sin(_rotationCounter);

        transform.Rotate(Vector3.back, angle * 0.3f);

        if (angle >= Mathf.PI * 2)
        {
            angle = -Mathf.PI * 2;
        }
    }
}
