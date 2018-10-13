using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleHandler : MonoBehaviour {

    public void CloseButtonHandler()
    {
        Application.Quit();
    }

    public void PlayButtonHandler()
    {
        SceneManager.LoadScene("combat");
    }
}
