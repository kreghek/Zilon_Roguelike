using System;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

public class ScoresHandler : MonoBehaviour
{
    public ScoresTableRow ScoresTableRowPrefab;
    public Transform ScoreRecordParent;

    [Inject]
    private readonly ScoreStorage _scoreStorage;

    public void Awake()
    {
        ScoresRecord[] scoreRecords;

        try
        {
            scoreRecords = _scoreStorage.ReadScores();
        }
        catch (Exception exception)
        {
            scoreRecords = new ScoresRecord[0];
            Debug.LogError("Не удалось выполнить чтение результатов из БД\n" + exception.ToString());
        }

        var parentRect = ScoreRecordParent.GetComponent<RectTransform>();
        var rowCount = scoreRecords.Length;
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, (60 + 5) * rowCount);

        foreach (var record in scoreRecords)
        {
            var row = Instantiate(ScoresTableRowPrefab, ScoreRecordParent);
            row.Init(record.Number, record.Name, record.Scores, "[not impl]", "[not impl]");
        }
    }

    public void ToMainMenuButton_Handler()
    {
        SceneManager.LoadScene("title");
    }
}
