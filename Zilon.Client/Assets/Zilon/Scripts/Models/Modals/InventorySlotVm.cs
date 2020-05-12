using System;

using Assets.Zilon.Scripts.Models;
using Assets.Zilon.Scripts.Models.Modals;
using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

public class InventorySlotVm : MonoBehaviour, IPropItemViewModel, IPropViewModelDescription
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
            var prop = Actor.Person.GetModule<IEquipmentModule>()[SlotIndex];
            return prop;
        }
    }

    public event EventHandler Click;
    public event EventHandler MouseEnter;
    public event EventHandler MouseExit;
    public event EventHandler<PropDraggingStateEventArgs> DraggingStateChanged;

    public bool SelectAsDrag;


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
        if (IconImage == null)
        {
            return;
        }

        var currentEquipment = Actor.Person.GetModule<IEquipmentModule>()[SlotIndex];
        if (currentEquipment != null)
        {
            SetEquipmentIcon(currentEquipment);
        }
        else
        {
            SetEmptySlotIcon();
        }
    }

    private void SetEmptySlotIcon()
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

    private void SetEquipmentIcon(Equipment currentEquipment)
    {
        var iconSprite = CalcIcon(currentEquipment);
        IconImage.sprite = iconSprite;
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
        Actor.Person.GetModule<IEquipmentModule>().EquipmentChanged += EquipmentCarrierOnEquipmentChanged;
    }

    private void ClearEventHandlers()
    {
        Actor.Person.GetModule<IEquipmentModule>().EquipmentChanged -= EquipmentCarrierOnEquipmentChanged;
    }

    public void SetDraggingState(bool value)
    {
        SelectAsDrag = value;

        if (value)
        {
            IconImage.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            IconImage.color = new Color(1, 1, 1, 1f);
        }

        DraggingStateChanged?.Invoke(this, new PropDraggingStateEventArgs(value));
    }

    private Sprite CalcIcon(IProp prop)
    {
        var schemeSid = prop.Scheme.Sid;
        if (prop.Scheme.IsMimicFor != null)
        {
            schemeSid = prop.Scheme.IsMimicFor;
        }

        var iconSprite = Resources.Load<Sprite>($"Icons/props/{schemeSid}");
        return iconSprite;
    }
}