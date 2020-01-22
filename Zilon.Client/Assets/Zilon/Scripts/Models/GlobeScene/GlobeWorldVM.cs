using UnityEngine;

public class GlobeWorldVM : MonoBehaviour
{
    public MapLocation LocationPrefab;
    public MapLocationConnector ConnectorPrefab;
    public GroupVM HumanGroupPrefab;
    public GlobalFollowCamera Camera;
    public SceneLoader SectorSceneLoader;
    public SceneLoader GlobeSceneLoader;
    public GameObject MapBackground;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members",
        Justification = "Неявно используется платформой Unity")]
    private void Start()
    {
        
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members",
        Justification = "Неявно используется платформой Unity")]
    private void StartLoadScene()
    {
        SectorSceneLoader.LoadScene();
    }
}