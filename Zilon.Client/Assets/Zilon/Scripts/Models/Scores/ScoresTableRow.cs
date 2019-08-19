using System;

using UnityEngine;
using UnityEngine.UI;

public class ScoresTableRow : MonoBehaviour
{
    public Text NumberText;

    public Text NameText;

    public Text ScoresText;

    public Text DeathReasonText;

    public Text ArcheventsText;

    public void Init(int number, string name, int scores, string deathReason, string archievents)
    {
        archievents = "nothing";

        NumberText.text = $"#{number}";
        NameText.text = name;
        ScoresText.text = $"Scores: {scores}";
        DeathReasonText.text = $"Death Reason:{Environment.NewLine}{deathReason}";
        ArcheventsText.text = archievents;
    }
}
