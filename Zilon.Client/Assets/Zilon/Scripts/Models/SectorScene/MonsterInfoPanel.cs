using System.Linq;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;

public class MonsterInfoPanel : MonoBehaviour
{
    public GameObject PaneContent;

    [Inject] private readonly UiSettingService _uiSettingService;
    [Inject] private readonly ISectorUiState _playerState;

    public Text MonsterNameText;
    public Text MonsterHpText;
    public Text MonsterDefencesText;

    public void Start()
    {
        PaneContent.SetActive(false);
    }

    public void Update()
    {
        if (_playerState.HoverViewModel is IActorViewModel actorViewModel)
        {
            if (actorViewModel.Actor.Person is MonsterPerson monsterPerson)
            {
                ShowMonsterInfo(monsterPerson);
            }
        }
        else
        {
            HideMonsterInfo();
        }
    }

    private void HideMonsterInfo()
    {
        PaneContent.SetActive(false);

        MonsterNameText.text = string.Empty;
        MonsterHpText.text = string.Empty;
        MonsterDefencesText.text = string.Empty;
    }

    private void ShowMonsterInfo(MonsterPerson monsterPerson)
    {
        PaneContent.SetActive(true);

        var currentLanguage = _uiSettingService.CurrentLanguage;
        SetMonsterName(monsterPerson, currentLanguage);
        SetMonsterStats(monsterPerson, currentLanguage);
    }

    private void SetMonsterStats(MonsterPerson monsterPerson, Language currentLanguage)
    {
        var hpStat = monsterPerson.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
        if (hpStat != null)
        {
            var monsterHealthStateKey = HealthHelper.GetHealthStateKey(hpStat);
            var monsterHealthState = StaticPhrases.GetValue($"state-hp-{monsterHealthStateKey}", currentLanguage);
            MonsterHpText.text = monsterHealthState;
        }
    }

    private void SetMonsterName(MonsterPerson monsterPerson, Language currentLanguage)
    {
        var monsterName = monsterPerson.Scheme.Name;
        var monsterLocalizedName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, monsterName);
        MonsterNameText.text = monsterLocalizedName;
    }
}
