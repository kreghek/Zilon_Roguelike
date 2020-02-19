using System;

using Assets.Zilon.Scripts;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

public class ScoreModalBody : MonoBehaviour, IModalWindowHandler
{
    public Text TotalScoreText;

    public Text DetailsText;

    public InputField NameInput;

    [Inject]
    private readonly IScoreManager _scoreManager;

    [Inject]
    private readonly ScoreStorage _scoreStorage;

    public string Caption => "Scores";

    public event EventHandler Closed;

    public void Init()
    {
        NameInput.text = "Безымянный бродяга";

        // TODO Сделать анимацию - плавное накручивание очков через Lerp от инта
        TotalScoreText.text = _scoreManager.BaseScores.ToString();

        DetailsText.text = TextSummaryHelper.CreateTextSummary(_scoreManager.Scores);
    }

    public void ApplyChanges()
    {
        var name = NameInput.text;

        var scores = _scoreManager.Scores;
        try
        {
            _scoreStorage.AppendScores(name, scores);
        }
        catch (Exception exception)
        {
            Debug.LogError("Не удалось выполнить запись результатов в БД\n" + exception.ToString());
        }

        SceneManager.LoadScene("scores");
    }

    public void CancelChanges()
    {
        throw new NotSupportedException();
    }
}
