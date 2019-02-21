using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

public class PropInfoPopup : MonoBehaviour
{
    [Inject] ISchemeService _schemeService;

    public Text NameText;
    public Text StatText;

    public PropItemVm PropViewModel { get; set; }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPropViewModel(PropItemVm propViewModel)
    {
        PropViewModel = propViewModel;

        if (propViewModel != null)
        {
            gameObject.SetActive(true);
            ShowPropStats(propViewModel);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowPropStats(PropItemVm propViewModel)
    {
        NameText.text = propViewModel.Prop.Scheme.Name.En ?? propViewModel.Prop.Scheme.Name.Ru;
        StatText.text = null;

        switch (propViewModel.Prop)
        {
            case Equipment equipment:
                foreach (var sid in propViewModel.Prop.Scheme.Equip.ActSids)
                {
                    var act = _schemeService.GetScheme<ITacticalActScheme>(sid);
                    StatText.text = $"Efficient: {act.Stats.Efficient.Count}D{act.Stats.Efficient.Dice}";
                }
                break;

            case Resource resource:
                if (resource.Scheme.Use != null)
                {
                    var rules = string.Join("\n", resource.Scheme.Use.CommonRules.Select(x => $"{x.Type}:{x.Level}"));
                    StatText.text = rules;
                }

                break;
        }
    }

    public void FixedUpdate()
    {
        if (PropViewModel != null)
        {
            
            GetComponent<RectTransform>().position = PropViewModel.GetComponent<RectTransform>().position 
                + new Vector3(0.4f, -0.4f);
        }
    }
}