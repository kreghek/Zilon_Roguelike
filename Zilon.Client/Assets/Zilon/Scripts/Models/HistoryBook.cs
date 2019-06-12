using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.WorldGeneration;

public class HistoryBook : MonoBehaviour
{
    public Text HistoryText;

    public void SetHistory(GlobeGenerationHistory globeGenerationHistory)
    {
        HistoryText.text = string.Empty;

        var iterationHistory = globeGenerationHistory.Items.GroupBy(x => x.Iteration).OrderBy(x => x.Key);

        foreach (var iterationHistoryGroup in iterationHistory)
        {
            HistoryText.text += $"{iterationHistoryGroup.Key} iteration" + Environment.NewLine;
            foreach (var historyItem in iterationHistoryGroup)
            {
                HistoryText.text += historyItem.Event + Environment.NewLine;
            }
        }
    }
}
