using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;

public class MonsterInfoPanel : MonoBehaviour
{
    [Inject] private readonly IPlayerState _playerState;

    public Text MonsterNameText;
    public Text MonsterHpText;

    public void FixedUpdate()
    {
        if (_playerState.HoverViewModel is IActorViewModel actorViewModel)
        {
            if (actorViewModel.Actor.Person is MonsterPerson monsterPerson)
            {
                MonsterNameText.text = monsterPerson.Scheme.Name.En ?? monsterPerson.Scheme.Name.Ru;

                var hpStat = monsterPerson.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
                if (hpStat != null)
                {
                    MonsterHpText.text = $"Hp: {hpStat.Value}/{hpStat.Range.Max}";
                }
            }
        }
        else
        {
            MonsterNameText.text = string.Empty;
            MonsterHpText.text = string.Empty;
        }
    }
}
