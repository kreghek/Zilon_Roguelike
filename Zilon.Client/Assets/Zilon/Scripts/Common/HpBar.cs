using System.Linq;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Players;

/// <summary>
/// Скрипт для визуализации полоски здоровья.
/// </summary>
public class HpBar : MonoBehaviour
{
    public Image BarImage;
    public Text Text;

    [NotNull] [Inject] private readonly HumanPlayer _player;


    public void Update()
    {
        if (_player.MainPerson == null)
        {
            // Главного персонажа может не быть, потому что персонажа ещё не создали.
            BarImage.fillAmount = 0;

            return;
        }

        var person = _player.MainPerson;

        var hpStat = person.Survival.Stats.Single(x => x.Type == SurvivalStatType.Health);

        var hpPercentage = CalcPercentage(hpStat.Value, hpStat.Range.Max);

        BarImage.fillAmount = hpPercentage;

        Text.text = $"{hpStat.Value}/{hpStat.Range.Max}";
    }

    private float CalcPercentage(float currentHp, float maxHp)
    {
        if (maxHp <= 0)
        {
            return 0;
        }

        return currentHp / maxHp;
    }
}
