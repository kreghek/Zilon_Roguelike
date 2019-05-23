using UnityEngine;

public class GlobeUiHandler : MonoBehaviour
{
    public SceneLoader SectorSceneLoader;

    public void EnterButtonHandler()
    {
        SectorSceneLoader.LoadScene();
    }
}
