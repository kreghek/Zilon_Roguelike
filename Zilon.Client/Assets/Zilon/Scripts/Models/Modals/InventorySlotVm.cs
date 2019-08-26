using System;

using Assets.Zilon.Scripts.Models;
using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

public class InventorySlotVm : MonoBehaviour, IPropViewModelDescription
{
    [Inject] private readonly ICommandManager _comamndManager;
    [Inject] private readonly IInventoryState _inventoryState;
    [Inject] private readonly SpecialCommandManager _specialCommandManager;

    [NotNull] private ICommand _equipCommand;

    public IActor Actor { get; set; }
    public int SlotIndex;

    public GameObject DenyBorder;
    public Image IconImage;
    public EquipmentSlotTypes SlotTypes;
    public Sprite[] TypeBackgrounds;

    public Vector3 Position => GetComponent<RectTransform>().position;
    public IProp Prop
    {
        get
        {
            var prop = Actor.Person.EquipmentCarrier[SlotIndex];
            return prop;
        }
    }

    public event EventHandler Click;
    public event EventHandler MouseEnter;
    public event EventHandler MouseExit;

    public void Start()
    {
        _equipCommand = _specialCommandManager.GetEquipCommand(SlotIndex);

        UpdateSlotIcon();
        InitEventHandlers();
    }

    public void OnDestroy()
    {
        ClearEventHandlers();
    }

    private void EquipmentCarrierOnEquipmentChanged(object sender, EquipmentChangedEventArgs e)
    {
        UpdateSlotIcon();
        _inventoryState.SelectedProp = null;
    }

    private void UpdateSlotIcon()
    {
        var currentEquipment = Actor.Person.EquipmentCarrier[SlotIndex];
        if (currentEquipment != null)
        {
            if (IconImage != null)
            {
                IconImage.sprite = Resources.Load<Sprite>($"Icons/props/{currentEquipment.Scheme.Sid}");
            }
        }
        else
        {
            if (IconImage != null)
            {
                switch (SlotTypes)
                {
                    case EquipmentSlotTypes.Head:
                        IconImage.sprite = TypeBackgrounds[0];
                        break;

                    case EquipmentSlotTypes.Body:
                        IconImage.sprite = TypeBackgrounds[1];
                        break;

                    case EquipmentSlotTypes.Hand:
                        IconImage.sprite = TypeBackgrounds[2];
                        break;

                    case EquipmentSlotTypes.Aux:
                        IconImage.sprite = TypeBackgrounds[3];
                        break;

                    default:
                        throw new InvalidOperationException($"Неизвестный тип слота {SlotTypes}.");
                }
            }
        }
    }

    public void FixedUpdate()
    {
        var canEquip = _equipCommand.CanExecute();
        var selectedProp = _inventoryState.SelectedProp;
        var denySlot = !canEquip && selectedProp != null;
        DenyBorder.SetActive(denySlot);
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

    public void ApplyEquipment()
    {
        _comamndManager.Push(_equipCommand);
    }


    private void InitEventHandlers()
    {
        Actor.Person.EquipmentCarrier.EquipmentChanged += EquipmentCarrierOnEquipmentChanged;
    }

    private void ClearEventHandlers()
    {
        Actor.Person.EquipmentCarrier.EquipmentChanged -= EquipmentCarrierOnEquipmentChanged;
    }
}