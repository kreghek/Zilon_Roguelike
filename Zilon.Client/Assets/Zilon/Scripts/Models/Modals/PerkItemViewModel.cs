﻿using System;

using Assets.Zilon.Scripts.Models;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core;
using Zilon.Core.Client.Sector;
using Zilon.Core.Persons;

/// <summary>
/// Модель перка.
/// </summary>
/// <remarks>
/// Сейчас используется только в окне персонажа.
/// </remarks>
public sealed class PerkItemViewModel : MonoBehaviour, IPerkViewModel, IPerkViewModelDescription
{
    public Text LevelText;
    public Image IconImage;
    public Image SelectedBorder;

    public Vector3 Position => GetComponent<RectTransform>().position;
    public IPerk Perk { get; private set; }

    public event EventHandler Click;
    public event EventHandler MouseEnter;
    public event EventHandler MouseExit;

    public void Init(IPerk perk)
    {
        Perk = perk;

        var iconSprite = CalcIcon(perk);

        IconImage.sprite = iconSprite;

        if (perk.CurrentLevel != null)
        {
            LevelText.gameObject.SetActive(true);
            var totalLevel = PerkHelper.ConvertLevelSubsToTotal(perk.Scheme, perk.CurrentLevel.Primary, perk.CurrentLevel.Sub);
            LevelText.text = totalLevel.ToString();
        }
        else
        {
            LevelText.gameObject.SetActive(false);
        }
    }

    public void Click_Handler()
    {
        Click?.Invoke(this, new EventArgs());
    }

    public void OnMouseEnter()
    {
        MouseEnter?.Invoke(this, new EventArgs());
    }

    public void OnMouseExit()
    {
        MouseExit?.Invoke(this, new EventArgs());
    }

    private Sprite CalcIcon(IPerk perk)
    {
        var iconSprite = Resources.Load<Sprite>($"Icons/perks/{perk.Scheme.Sid}");
        if (iconSprite is null)
        {
            iconSprite = Resources.Load<Sprite>($"Icons/perks/default");
        }

        return iconSprite;
    }
}
