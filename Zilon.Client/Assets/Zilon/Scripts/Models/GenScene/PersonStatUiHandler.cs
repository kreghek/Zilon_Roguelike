using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

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

        var person = PersonFollower.FollowedPerson.Actor.Person;
        var survivalStatStrings = person.Survival.Stats.Select(x => $"{x.Type}: {x.Value}");
        foreach (var survivalStatString in survivalStatStrings)
        {
            sb.AppendLine(survivalStatString);
        }

        sb.AppendLine("=== Effects ===");

        foreach (var effect in person.Effects.Items)
        {
            sb.AppendLine(effect.ToString());
        }

        StatText.text = sb.ToString();
    }
}
