﻿using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Models;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

public class PropInfoPopup : MonoBehaviour
{
    [Inject] private ISchemeService _schemeService;

    public Text NameText;
    public Text StatText;

    public IPropViewModelDescription PropViewModel { get; set; }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPropViewModel(IPropViewModelDescription propViewModel)
    {
        PropViewModel = propViewModel;

        if (propViewModel?.Prop != null)
        {
            gameObject.SetActive(true);
            ShowPropStats(propViewModel);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowPropStats(IPropViewModelDescription propViewModel)
    {
        NameText.text = propViewModel.Prop.Scheme.Name.En ?? propViewModel.Prop.Scheme.Name.Ru;
        StatText.text = null;

        var propScheme = propViewModel.Prop.Scheme;

        switch (propViewModel.Prop)
        {
            case Equipment equipment:

                var descriptionLines = new List<string>();

                if (propScheme.Equip.ActSids != null)
                {

                    var actDescriptions = new List<string>();
                    foreach (var sid in propScheme.Equip.ActSids)
                    {
                        var act = _schemeService.GetScheme<ITacticalActScheme>(sid);
                        var actName = act.Name.En ?? act.Name.Ru;
                        var efficient = $"{act.Stats.Efficient.Count}D{act.Stats.Efficient.Dice}";
                        var actImpact = act.Stats.Offense.Impact;
                        descriptionLines.Add($"{actName}: {actImpact} {efficient} ({act.Stats.Offense.ApRank} rank)");
                    }
                }

                if (propScheme.Equip.Armors != null)
                {
                    foreach (var armor in propScheme.Equip.Armors)
                    {
                        descriptionLines.Add($"Protects: {armor.Impact} ({armor.ArmorRank} rank): {armor.AbsorbtionLevel}");
                    }
                }

                StatText.text = string.Join("\n", descriptionLines);

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

            GetComponent<RectTransform>().position = PropViewModel.Position
                + new Vector3(0.4f, -0.4f);
        }
    }
}