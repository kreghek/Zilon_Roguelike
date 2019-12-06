using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;

public class MonsterInfoPanel : MonoBehaviour
{
    public GameObject PaneContent;

    [Inject] private readonly ISectorUiState _playerState;

    public Text MonsterNameText;
    public Text MonsterHpText;
    public Text MonsterDefencesText;

    public void Start()
    {
        PaneContent.SetActive(false);
    }

    public void FixedUpdate()
    {
        if (_playerState.HoverViewModel is IActorViewModel actorViewModel)
        {
            if (actorViewModel.Actor.Person is MonsterPerson monsterPerson)
            {
                PaneContent.SetActive(true);

                MonsterNameText.text = monsterPerson.Scheme.Name?.En ?? monsterPerson.Scheme.Name?.Ru ?? "[No Name]";

                var hpStat = monsterPerson.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
                if (hpStat != null)
                {
                    var monsterHealthString = GetHealthString(hpStat);
                    MonsterHpText.text = monsterHealthString;
                }

                MonsterDefencesText.text = string.Empty;
                if (monsterPerson.CombatStats?.DefenceStats?.Defences != null)
                {
                    foreach (var defenceItem in monsterPerson.CombatStats.DefenceStats.Defences)
                    {
                        MonsterDefencesText.text += $"{defenceItem.Type}: {defenceItem.Level}\n";
                    }
                }
            }
        }
        else
        {
            PaneContent.SetActive(false);

            MonsterNameText.text = string.Empty;
            MonsterHpText.text = string.Empty;
            MonsterDefencesText.text = string.Empty;
        }
    }

    private string GetHealthString(SurvivalStat hpStat)
    {
        var hpPercentage = hpStat.ValueShare;

        if (hpPercentage >= 0.95f)
        {
            return "Healthy";
        }
        if (0.75f <= hpPercentage && hpPercentage < 0.95f)
        {
            return "Slightly Injured";
        }
        else if (0.5f <= hpPercentage && hpPercentage < 0.75f)
        {
            return "Wounded";
        }
        else if (0.25f <= hpPercentage && hpPercentage < 0.5f)
        {
            return "Badly Wounded";
        }
        else
        {
            return "At Death";
        }
    }
}
