using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

public class SaveLastResultHandler : MonoBehaviour
{
    [Inject]
    private readonly IScoreManager _scoreManager;

    [Inject]
    private readonly ScoreStorage _scoreStorage;

    public Text ScoreText;
    public Text SummaryText;
    public InputField NameInput;

    private void Awake()
    {
        ScoreText.text = $"Очки: {_scoreManager.BaseScores}";

        var summaryText = TextSummaryHelper.CreateTextSummary(_scoreManager.Scores);
        SummaryText.text = summaryText;
    }

    public void SaveResults()
    {
        var name = NameInput.text;

        var scores = _scoreManager.Scores;
        try
        {
            _scoreStorage.AppendScores(name, scores);
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Не удалось выполнить запись результатов в БД\n" + exception.ToString());
        }

        SceneManager.LoadScene("Scores");
    }
}
