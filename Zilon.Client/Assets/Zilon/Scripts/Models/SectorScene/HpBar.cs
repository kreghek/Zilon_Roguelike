using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;

public class HpBar : MonoBehaviour
{
    public Image BarImage;
    public Text Text;

    [NotNull] [Inject] private readonly ISectorUiState _playerState;


    public void Update()
    {
        if (_playerState.ActiveActor == null)
        {
            // Активного актёра может не быть, потому что игрока убили и занулили это свойство.
            BarImage.fillAmount = 0;

            return;
        }

        var actorVm = _playerState.ActiveActor;
        var person = actorVm.Actor.Person;


        var hpStat = person.Survival.Stats.Single(x => x.Type == SurvivalStatType.Health);


        var hpPercentage = CalcPercentage(hpStat.Value, hpStat.Range.Max);

        BarImage.fillAmount = hpPercentage;

        Text.text = $"{hpStat.Value}/{hpStat.Range.Max}";
    }

    private float CalcPercentage(float actorHp, float personHp)
    {
        if (personHp <= 0)
        {
            return 0;
        }

        return actorHp / personHp;
    }
}
