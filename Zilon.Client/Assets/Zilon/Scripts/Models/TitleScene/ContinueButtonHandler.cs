using Assets.Zilon.Scripts.Models.TitleScene;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;
using Color = UnityEngine.Color;

public class ContinueButtonHandler : MonoBehaviour
{
    public Button ContinueButton;

    public Image IconImage;

    public Text ContinueText;

    [Inject] private readonly IWorldManager _globeManager;
    [Inject] private readonly IScoreManager _scoreManager;
    [Inject] private readonly HumanPlayer _humanPlayer;

    [Inject]
    ProgressStorageService _progressStorageService;

    // Start is called before the first frame update
    void Start()
    {
        if (!_progressStorageService.HasSaves())
        {
            ContinueButton.interactable = false;
            ContinueText.color = Color.Lerp(ContinueText.color, new Color(0, 0, 0, 0), 0.5f);
            IconImage.color = Color.Lerp(IconImage.color, new Color(0, 0, 0, 0), 0.5f);
            _progressStorageService.Destroy();
        }
    }

    public void ContinueHandler()
    {
        GameProgressHelper.ResetGameState(_globeManager, _scoreManager, _humanPlayer);
        SceneManager.LoadScene("globe");
    }
}
