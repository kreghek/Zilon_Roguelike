using Assets.Zilon.Scripts.Models.TitleScene;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Scoring;

public class NewGameButtonHandler : MonoBehaviour
{
    [Inject] private readonly IScoreManager _scoreManager;
    [Inject] private readonly IPlayer _humanPlayer;

    public void PlayHandler()
    {
        GameProgressHelper.ResetGameState(_scoreManager, _humanPlayer);
        SceneManager.LoadScene("globe-selection");
    }
}
