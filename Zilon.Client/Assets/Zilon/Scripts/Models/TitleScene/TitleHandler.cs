using Assets.Zilon.Scripts.Models.TitleScene;

using UnityEngine;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Scoring;
using Zilon.Core.World;

public class TitleHandler : MonoBehaviour
{
    [Inject] private readonly IWorldManager _globeManager;
    [Inject] private readonly IScoreManager _scoreManager;
    [Inject] private readonly HumanPlayer _humanPlayer;

    public void Start()
    {
        GameProgressHelper.ResetGameState(_globeManager, _scoreManager, _humanPlayer);
    }

    public void CloseButtonHandler()
    {
        Application.Quit();
    }

    public void OpenRepoUrlHandler()
    {
        Application.OpenURL("https://github.com/kreghek/Zilon_Roguelike");
    }

    public void OpenVkUrlHandler()
    {
        Application.OpenURL("https://vk.com/last_imperial_vagabond");
    }

    public void OpenBlogUrlHandler()
    {
        Application.OpenURL("https://lastimperialvagabond.home.blog/");
    }
}
