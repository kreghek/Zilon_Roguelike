using UnityEngine;

public class ModalDialog : MonoBehaviour
{
    public GameObject Body;
    
    public void Close()
    {
        Destroy(gameObject);
    }
}