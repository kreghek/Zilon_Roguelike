using Assets.Zilon.Scripts.Models.TitleScene;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Scoring;

public class NewGameButtonHandler : MonoBehaviour
{
    [Inject] private readonly IScoreManager _scoreManager;
    [Inject] private readonly IPlayer _humanPlayer;

    [Inject]
    ProgressStorageService _progressStorageService;

    // Start is called before the first frame update
    public void PlayHandler()
    {
        GameProgressHelper.ResetGameState(_scoreManager, _humanPlayer);
        _progressStorageService.Destroy();
        SceneManager.LoadScene("globe-selection");
    }
}
