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
        var scoreRecords = _scoreStorage.ReadScores();

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
