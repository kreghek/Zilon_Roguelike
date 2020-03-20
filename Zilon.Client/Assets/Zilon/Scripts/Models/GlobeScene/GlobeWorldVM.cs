using UnityEngine;

using Zenject;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;
using Zilon.Core.Commands.Globe;
using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Scoring;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

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