using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleHandler : MonoBehaviour
{

    public void CloseButtonHandler()
    {
        Application.Quit();
    }

    public void PlayButtonHandler()
    {
        SceneManager.LoadScene("combat");
    }

    public void OpenRepoUrlHandler()
    {
        Application.OpenURL("https://github.com/kreghek/Zilon_Roguelike");
    }
}
