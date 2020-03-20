using System;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using Zilon.Core.Persons;

public class PersonStatUiHandler : MonoBehaviour
{
    public PersonFollower PersonFollower;
    public Text StatText;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "UNT0001:Empty Unity message",
        Justification = "Не явно используется Юнити.")]
    private void Update()
    {
        if (!PersonFollower.IsFollowing)
        {
            StatText.text = string.Empty;
            return;
        }

        var sb = new StringBuilder();

        //TODO
        sb.AppendLine((PersonFollower.FollowedPerson.Actor.Person as HumanPerson).Name);
        sb.AppendLine("=== Stats ===");

        var person = PersonFollower.FollowedPerson.Actor.Person;
        foreach (var survivalStat in person.Survival.Stats)
        {
            var statPercent = (int)Math.Round(survivalStat.ValueShare * 100);
            sb.AppendLine($"{survivalStat.Type}: {statPercent}%");
        }

        if (person.Effects.Items.Any())
        {
            sb.AppendLine("=== Effects ===");

            foreach (var effect in person.Effects.Items)
            {
                sb.AppendLine(effect.ToString());
            }
        }

        StatText.text = sb.ToString();
    }
}
