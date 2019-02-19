using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.SceneManagement;

public class WinModalBody : MonoBehaviour, IModalWindowHandler
{
    public string Caption => "Critical error!!";

    public void ApplyChanges()
    {
        SceneManager.LoadScene(0);
    }

    public void CancelChanges()
    {
        throw new System.NotImplementedException();
    }
}
