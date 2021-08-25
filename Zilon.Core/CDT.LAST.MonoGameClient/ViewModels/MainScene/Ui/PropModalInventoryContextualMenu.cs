﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class PropModalInventoryContextualMenu : PropModalInventoryContextualMenuBase
    {
        private readonly IEquipmentModule _equipmentModule;
        private readonly IServiceProvider _serviceProvider;

        private IProp? _prop;

        public PropModalInventoryContextualMenu(Point position, IEquipmentModule equipmentModule,
            IUiContentStorage uiContentStorage,
            IServiceProvider serviceProvider) : base(position, uiContentStorage)
        {
            _equipmentModule = equipmentModule;
            _serviceProvider = serviceProvider;
        }

        protected override TextButton[] InitItems(IProp prop)
        {
            _prop = prop;

            var inventoryState = _serviceProvider.GetRequiredService<IInventoryState>();
            inventoryState.SelectedProp = new PropViewModel(prop);

            var list = new List<TextButton>();

            var useCommand = _serviceProvider.GetRequiredService<UseSelfCommand>();

            var commandPool = _serviceProvider.GetRequiredService<ICommandPool>();

            switch (prop)
            {
                case Equipment:
                    InitItemsForEquipment(list, commandPool);
                    break;

                case Resource:
                    InitItemsForResource(list, useCommand, commandPool);
                    break;

                default:
                    Debug.Fail($"Unknown type {prop.GetType()} of the prop.");
                    break;
            }

            return list.ToArray();
        }

        private static string GetInventoryMenuItemContent(IProp prop)
        {
            switch (prop.Scheme.Sid)
            {
                case "med-kit":
                    return UiResources.HealCommandButtonTitle;

                case "water-bottle":
                    return UiResources.DrinkCommandButtonTitle;

                case "packed-food":
                    return UiResources.EatCommandButtonTitle;

                default:
                    Debug.Fail("Every consumable must have definative command title.");
                    return UiResources.UseCommandButtonTitle;
            }
        }

        private static string GetSlotTitle(EquipmentSlotTypes types)
        {
            switch (types)
            {
                case EquipmentSlotTypes.Hand:
                    return UiResources.SlotHand;

                case EquipmentSlotTypes.Head:
                    return UiResources.SlotHead;

                case EquipmentSlotTypes.Body:
                    return UiResources.SlotBody;

                case EquipmentSlotTypes.Aux:
                    return UiResources.SlotAux;

                default:
                    Debug.Fail("All slot types must have name.");
                    return "<Unknown>";
            }
        }

        private void InitItemsForEquipment(List<TextButton> list, ICommandPool commandPool)
        {
            for (var slotIndex = 0; slotIndex < _equipmentModule.Slots.Length; slotIndex++)
            {
                var slot = _equipmentModule.Slots[slotIndex];
                var equipCommand = _serviceProvider.GetRequiredService<EquipCommand>();
                equipCommand.SlotIndex = slotIndex;
                if (equipCommand.CanExecute().IsSuccess)
                {
                    var slotTitle = GetSlotTitle(slot.Types);
                    var equipButtonTitle =
                        string.Format(UiResources.EquipInSlotTemplateCommandButton, slotTitle);
                    var equipButton = new TextButton(equipButtonTitle,
                        _uiContentStorage.GetMenuItemTexture(),
                        _uiContentStorage.GetMenuItemFont(),
                        new Rectangle(
                            MENU_MARGIN + _position.X,
                            MENU_MARGIN + _position.Y + (list.Count * MENU_ITEM_HEIGHT),
                            MENU_WIDTH,
                            MENU_ITEM_HEIGHT));
                    equipButton.OnClick += (s, e) =>
                    {
                        commandPool.Push(equipCommand);
                        CloseMenu();
                        IsCommandUsed = true;
                    };
                    list.Add(equipButton);
                }
            }
        }

        private void InitItemsForResource(List<TextButton> list, UseSelfCommand useCommand, ICommandPool commandPool)
        {
            if (_prop is null)
            {
                return;
            }

            if (!useCommand.CanExecute().IsSuccess)
            {
                return;
            }

            var localizedCommandTitle = GetInventoryMenuItemContent(_prop);
            var useButton = new TextButton(localizedCommandTitle,
                _uiContentStorage.GetMenuItemTexture(),
                _uiContentStorage.GetMenuItemFont(),
                new Rectangle(
                    MENU_MARGIN + _position.X,
                    MENU_MARGIN + _position.Y,
                    MENU_WIDTH,
                    MENU_ITEM_HEIGHT));
            useButton.OnClick += (s, e) =>
            {
                commandPool.Push(useCommand);
                CloseMenu();
                IsCommandUsed = true;
            };
            list.Add(useButton);
        }
    }
}