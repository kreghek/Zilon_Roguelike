using System;

using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Tactics;

public class ScoreModalBody : MonoBehaviour, IModalWindowHandler
{
    public Text TotalScoreText;

    public Text DetailsText;

    [Inject] readonly IScoreManager _scoreManager;

    [Inject] readonly HumanPlayer _humanPlayer;

    public string Caption => "Scores";

    public event EventHandler Closed;

    public void Init()
    {
        // TODO Сделать анимацию - плавное накручивание очков через Lerp от инта
        TotalScoreText.text = _scoreManager.BaseScores.ToString();

        if (_humanPlayer.MainPerson.Survival.IsDead)
        {
            DetailsText.text = "YOU DIED" + "\n" + "\n";
        }

        if (_scoreManager.Achievements.HasFlag(ScoreAchievements.HomeFound))
        {
            DetailsText.text = "HOME FOUND" + "\n" + "\n";
        }

        DetailsText.text += "=== You survived ===" + "\n";
        var minutesTotal = _scoreManager.Turns * 2;
        var hoursTotal = minutesTotal / 60f;
        var daysTotal = hoursTotal / 24f;
        var days = (int)daysTotal;
        var hours = (int)(hoursTotal - days * 24);

        DetailsText.text += $"{days} days {hours} hours" + "\n";
        Debug.Log($"Turns: {_scoreManager.Turns}");

        DetailsText.text += "=== You visited ===" + "\n";

        DetailsText.text += $"{_scoreManager.Places.Count} places" + "\n";

        foreach (var placeType in _scoreManager.PlaceTypes)
        {
            DetailsText.text += $"{placeType.Key.Name?.En ?? placeType.Key.Name?.Ru ?? placeType.Key.ToString()}: {placeType.Value} turns" + "\n";
        }

        DetailsText.text += "=== You killed ===" + "\n";
        foreach (var frag in _scoreManager.Frags)
        {
            DetailsText.text += $"{frag.Key.Name?.En ?? frag.Key.Name?.Ru ?? frag.Key.ToString()}: {frag.Value}" + "\n";
        }
    }

    public void ApplyChanges()
    {
        SceneManager.LoadScene("title");
    }

    public void CancelChanges()
    {
        SceneManager.LoadScene("title");
    }
}
