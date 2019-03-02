using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;

public class TitleHandler : MonoBehaviour
{
    [Inject] private readonly IWorldManager _globeManager;
    [Inject] private readonly IScoreManager _scoreManager;
    [Inject] private readonly HumanPlayer _humanPlayer;

    public void Start()
    {
        _globeManager.Globe = null;
        _humanPlayer.GlobeNode = null;
        _humanPlayer.MainPerson = null;
        _humanPlayer.SectorSid = null;
        _humanPlayer.Terrain = null;
        _scoreManager.ResetScores();
    }

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
