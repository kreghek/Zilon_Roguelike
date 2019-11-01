using UnityEngine;

public class ContainerPopupShadow : MonoBehaviour
{
    public GameObject DestroyObject;

    public void OnMouseDown()
    {
        Destroy(DestroyObject);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(DestroyObject);
        }
    }
}
