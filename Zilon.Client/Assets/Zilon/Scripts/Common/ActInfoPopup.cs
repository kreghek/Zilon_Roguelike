using Assets.Zilon.Scripts.Models;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Persons;

//TODO Попробовать объедининть с PerkInfoPopup
/// <summary>
/// Всплывающее окно с краткой информацией о предмете.
/// </summary>
/// <remarks>
/// Для подробной информации будет использовать отдельный модал.
/// </remarks>
public class ActInfoPopup : MonoBehaviour
{
    [Inject] private readonly UiSettingService _uiSettingService;

    public Text NameText;

    public IActViewModelDescription ActViewModel { get; set; }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPropViewModel(IActViewModelDescription viewModel)
    {
        ActViewModel = viewModel;

        if (viewModel?.Act != null)
        {
            gameObject.SetActive(true);
            ShowPropInfo(viewModel);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowPropInfo(IActViewModelDescription viewModel)
    {
        var act = viewModel.Act;

        NameText.text = GetPropDisplayName(act);
    }

    private string GetPropDisplayName(ITacticalAct propScheme)
    {
        var lang = _uiSettingService.CurrentLanguage;
        var name = propScheme.Scheme.Name;

        return LocalizationHelper.GetValueOrDefaultNoname(lang, name);
    }

    public void Update()
    {
        if (ActViewModel != null)
        {
            GetComponent<RectTransform>().position = ActViewModel.Position
                + new Vector3(0.4f, 0.6f);
        }
    }
}