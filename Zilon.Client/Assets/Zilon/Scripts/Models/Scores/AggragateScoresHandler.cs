using System;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class AggragateScoresHandler : MonoBehaviour
{
    public Text AvgScoresText;
    public Text MaxScoresText;
    public Text AvgFragsText;
    public Text MaxFragsText;
    public Text AvgLifetimeText;
    public Text MaxLifetimeText;

    [Inject]
    private readonly ScoreStorage _scoreStorage;

    public void Awake()
    {
        AggregateScores aggregareResults;

        try
        {
            aggregareResults = _scoreStorage.ReadAggregateScores();
        }
        catch (Exception exception)
        {
            aggregareResults = new AggregateScores();
            Debug.LogError("Не удалось выполнить чтение результатов из БД\n" + exception.ToString());
        }

        AvgScoresText.text = $"Avg Scores: {aggregareResults.AvgScores:F2}";
        MaxScoresText.text = $"Max Scores: {aggregareResults.MaxScores:F2}";

        AvgFragsText.text = $"Avg Frags: {aggregareResults.AvgFrags:F2}";
        MaxFragsText.text = $"Max Frags: {aggregareResults.MaxFrags:F2}";

        var avgLifetime = GetLifetime((int)aggregareResults.AvgTurns);
        AvgLifetimeText.text = $"Avg Lifetime: {avgLifetime}";
        var maxLifetime = GetLifetime((int)aggregareResults.MaxTurns);
        MaxLifetimeText.text = $"Max Lifetime: {maxLifetime}";
    }

    private string GetLifetime(int turns)
    {
        var minutesTotal = turns * 2;
        var hoursTotal = minutesTotal / 60f;
        var daysTotal = hoursTotal / 24f;
        var days = (int)daysTotal;
        var hours = (int)(hoursTotal - days * 24);

        return $"{days} days {hours} hours" + "\n";
    }
}
