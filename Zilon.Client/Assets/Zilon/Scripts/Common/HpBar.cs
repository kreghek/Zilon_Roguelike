using System;
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
    private const float CURRENT_HP_FILL_SPEED = 1.5f;

    private HumanPerson _lastHumanPerson;
    private float _lastPercentage;

    public Image BarImage;
    public Text Text;
    public HpBarHighlighter Highlighter;

    [NotNull] [Inject] private readonly HumanPlayer _player;


    public void FixedUpdate()
    {
        if (_player.MainPerson == null)
        {
            // Главного персонажа может не быть, потому что персонажа ещё не создали.
            BarImage.fillAmount = 0;

            return;
        }

        var person = _player.MainPerson;

        var hpStat = person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
        if (hpStat == null)
        {
            // У персонажа может не быть ХП.
            BarImage.fillAmount = 0;

            return;
        }

        var hpPercentage = CalcPercentage(hpStat.Value, hpStat.Range.Max);

        BarImage.fillAmount = Mathf.Lerp(BarImage.fillAmount, hpPercentage, Time.deltaTime * CURRENT_HP_FILL_SPEED);

        Text.text = $"{hpStat.Value}/{hpStat.Range.Max}";

        if (_lastHumanPerson != null && _lastHumanPerson == person)
        {
            if (_lastPercentage > hpPercentage)
            {
                HighlightHp();
            }
        }

        _lastHumanPerson = person;
        _lastPercentage = hpPercentage;
    }

    private void HighlightHp()
    {
        Highlighter.StartHighlighting();
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
