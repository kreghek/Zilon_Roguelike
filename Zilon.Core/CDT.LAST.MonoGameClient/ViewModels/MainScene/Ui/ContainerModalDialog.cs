﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class ContainerModalDialog : ModalDialogBase
    {
        private const int EQUIPMENT_ITEM_SIZE = 32;
        private const int EQUIPMENT_ITEM_SPACING = 2;

        private readonly string _containerName;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;
        private IStaticObject? _container;
        private ContainerModalTransferContextualMenu? _containerPropContextMenu;
        private InventoryUiItem[]? _currentContainerItems;
        private InventoryUiItem[]? _currentInventoryItems;
        private InventoryUiItem? _hoverContainerItem;

        private InventoryUiItem? _hoverInventoryItem;

        private ContainerModalTransferContextualMenu? _inventoryPropContextMenu;

        public ContainerModalDialog(ISectorUiState uiState, IUiContentStorage uiContentStorage,
            GraphicsDevice graphicsDevice,
            IServiceProvider serviceProvider) : base(
            uiContentStorage, graphicsDevice)
        {
            _uiState = uiState;
            _uiContentStorage = uiContentStorage;
            _serviceProvider = serviceProvider;

            _containerName = UiResources.FakeContainerName;
        }

        protected override void DrawContent(SpriteBatch spriteBatch)
        {
            //DrawInventory(spriteBatch);
            DrawContainer(spriteBatch);

            if (_inventoryPropContextMenu != null)
            {
                _inventoryPropContextMenu.Draw(spriteBatch);
            }
            else if (_containerPropContextMenu != null)
            {
                _containerPropContextMenu.Draw(spriteBatch);
            }

            //DrawInventoryHintIfSelected(spriteBatch);
            DrawContainerHintIfSelected(spriteBatch);
        }

        protected override void UpdateContent()
        {
            if (_inventoryPropContextMenu != null)
            {
                _inventoryPropContextMenu.Update();

                if (_inventoryPropContextMenu.IsClosed)
                {
                    if (_inventoryPropContextMenu.IsCommandUsed)
                    {
                        Close();
                    }

                    _inventoryPropContextMenu = null;
                }
            }
            else if (_containerPropContextMenu != null)
            {
                _containerPropContextMenu.Update();

                if (_containerPropContextMenu.IsClosed)
                {
                    if (_containerPropContextMenu.IsCommandUsed)
                    {
                        Close();
                    }

                    _containerPropContextMenu = null;
                }
            }

            UpdateInventory();
            UpdateContainer();

            var mouseState = Mouse.GetState();

            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            DetectHoverInventory(mouseRectangle);
            DetectHoverContainer(mouseRectangle);
        }

        internal void Init(IStaticObject container)
        {
            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                throw new InvalidOperationException("Active person must be selected before this dialog was opened.");
            }

            //InitInventory(person);

            InitContainerContent(container);
        }

        private void ContainerPropButton_OnClick(object? sender, EventArgs e)
        {
            if (_currentContainerItems is null)
            {
                throw new InvalidOperationException("Attempt to handle button click before InitInventory called.");
            }

            var clickedUiItem = _currentContainerItems.Single(x => x.Control == sender);
            var selectedProp = clickedUiItem.Prop;

            var mouseState = Mouse.GetState();

            var person = _uiState.ActiveActor?.Actor?.Person;

            if (person is null)
            {
                throw new InvalidOperationException("ISectorUiState must have active person assigned.");
            }

            var equipmentModule = person.GetModuleSafe<IEquipmentModule>();
            if (equipmentModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must be able to use equipment to shown in this dialog.");
            }

            if (_container is null)
            {
                throw new InvalidOperationException("Container is not defined. Initialization in not called.");
            }

            var contextMenu = new ContainerModalTransferContextualMenu(mouseState.Position,
                person.GetModule<IInventoryModule>(),
                _container.GetModule<IPropContainer>().Content,
                _uiContentStorage,
                _serviceProvider,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory,
                UiResources.TakePropInInventoryButtonTitle);

            contextMenu.Init(selectedProp);
            _containerPropContextMenu = contextMenu;
        }

        private void DetectHoverContainer(Rectangle mouseRectangle)
        {
            if (_currentContainerItems is null)
            {
                return;
            }

            var propUnderMouse = _currentContainerItems.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            _hoverContainerItem = propUnderMouse;
        }

        private void DetectHoverInventory(Rectangle mouseRectangle)
        {
            if (_currentInventoryItems is null)
            {
                return;
            }

            var propUnderMouse = _currentInventoryItems.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            _hoverInventoryItem = propUnderMouse;
        }

        private void DrawContainer(SpriteBatch spriteBatch)
        {
            if (_currentContainerItems is null)
            {
                return;
            }

            foreach (var item in _currentContainerItems)
            {
                item.Control.Draw(spriteBatch);

                var propTitle = PropHelper.GetPropTitle(item.Prop);
                if (item.Prop is Resource resource)
                {
                    propTitle += $" x{resource.Count}";
                }

                spriteBatch.DrawString(_uiContentStorage.GetButtonFont(), propTitle,
                    new Vector2(item.Control.Rect.Right + 2, item.Control.Rect.Top), Color.Wheat);
            }
        }

        private void DrawContainerHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_hoverContainerItem is null)
            {
                return;
            }

            var inventoryTitle = PropHelper.GetPropHintText(_hoverContainerItem.Prop);
            var hintTitleFont = _uiContentStorage.GetHintTitleFont();
            var titleTextSizeVector = hintTitleFont.MeasureString(inventoryTitle);

            const int HINT_TEXT_SPACING = 8;
            var hintRectangle = new Rectangle(
                _hoverContainerItem.UiRect.Left,
                _hoverContainerItem.UiRect.Bottom + EQUIPMENT_ITEM_SPACING,
                (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

            spriteBatch.DrawString(hintTitleFont, inventoryTitle,
                new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                Color.Wheat);
        }

        private void DrawInventory(SpriteBatch spriteBatch)
        {
            if (_currentInventoryItems is null)
            {
                return;
            }

            foreach (var item in _currentInventoryItems)
            {
                item.Control.Draw(spriteBatch);
            }
        }

        private void DrawInventoryHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_hoverInventoryItem is null)
            {
                return;
            }

            var inventoryTitle = PropHelper.GetPropHintText(_hoverInventoryItem.Prop);
            var hintTitleFont = _uiContentStorage.GetHintTitleFont();
            var titleTextSizeVector = hintTitleFont.MeasureString(inventoryTitle);

            const int HINT_TEXT_SPACING = 8;
            var hintRectangle = new Rectangle(
                _hoverInventoryItem.UiRect.Left,
                _hoverInventoryItem.UiRect.Bottom + EQUIPMENT_ITEM_SPACING,
                (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

            spriteBatch.DrawString(hintTitleFont, inventoryTitle,
                new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                Color.Wheat);
        }

        private void InitContainerContent(IStaticObject container)
        {
            _container = container;

            var currentContainerItemList = new List<InventoryUiItem>();

            var props = container.GetModule<IPropContainer>().Content.CalcActualItems().Take(8).ToArray();
            for (var itemIndex = 0; itemIndex < props.Length; itemIndex++)
            {
                var prop = props[itemIndex];

                var relativeY = itemIndex * (EQUIPMENT_ITEM_SIZE + EQUIPMENT_ITEM_SPACING);
                var buttonRect = new Rectangle(
                    ContentRect.Left,
                    ContentRect.Top + relativeY,
                    EQUIPMENT_ITEM_SIZE,
                    EQUIPMENT_ITEM_SIZE);

                var sid = prop.Scheme.Sid;
                if (string.IsNullOrEmpty(sid))
                {
                    Debug.Fail("All prop must have symbolic identifier (SID).");
                    sid = "EmptyPropIcon";
                }

                var propButton = new IconButton(
                    _uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetPropIconLayers(sid).First(),
                    buttonRect);
                propButton.OnClick += ContainerPropButton_OnClick;

                var uiItem = new InventoryUiItem(propButton, prop, itemIndex, buttonRect);

                currentContainerItemList.Add(uiItem);
            }

            _currentContainerItems = currentContainerItemList.ToArray();
        }

        private void InitInventory(IPerson person)
        {
            var inventoryModule = person.GetModuleSafe<IInventoryModule>();
            if (inventoryModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must be able to use equipment to shown in this dialog.");
            }

            var currentInventoryItemList = new List<InventoryUiItem>();
            foreach (var prop in inventoryModule.CalcActualItems())
            {
                var lastIndex = currentInventoryItemList.Count;
                var relativeX = lastIndex * (EQUIPMENT_ITEM_SIZE + EQUIPMENT_ITEM_SPACING);
                var buttonRect = new Rectangle(
                    relativeX + ContentRect.Left,
                    ContentRect.Top + EQUIPMENT_ITEM_SIZE,
                    EQUIPMENT_ITEM_SIZE,
                    EQUIPMENT_ITEM_SIZE);

                var sid = prop.Scheme.Sid;
                if (string.IsNullOrEmpty(sid))
                {
                    Debug.Fail("All prop must have symbolic identifier (SID).");
                    sid = "EmptyPropIcon";
                }

                var propButton = new IconButton(
                    _uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetPropIconLayers(sid).First(),
                    buttonRect);
                propButton.OnClick += InventoryPropButton_OnClick;

                var uiItem = new InventoryUiItem(propButton, prop, lastIndex, buttonRect);

                currentInventoryItemList.Add(uiItem);
            }

            _currentInventoryItems = currentInventoryItemList.ToArray();
        }

        private void InventoryPropButton_OnClick(object? sender, EventArgs e)
        {
            if (_currentInventoryItems is null)
            {
                throw new InvalidOperationException("Attempt to handle button click before InitInventory called.");
            }

            var clickedUiItem = _currentInventoryItems.Single(x => x.Control == sender);
            var selectedProp = clickedUiItem.Prop;

            var mouseState = Mouse.GetState();

            var person = _uiState.ActiveActor?.Actor?.Person;

            if (person is null)
            {
                throw new InvalidOperationException("ISectorUiState must have active person assigned.");
            }

            var equipmentModule = person.GetModuleSafe<IEquipmentModule>();
            if (equipmentModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must be able to use equipment to shown in this dialog.");
            }

            if (_container is null)
            {
                throw new InvalidOperationException("Container is not defined. Initialization in not called.");
            }

            var contextMenu = new ContainerModalTransferContextualMenu(mouseState.Position,
                person.GetModule<IInventoryModule>(),
                _container.GetModule<IPropContainer>().Content,
                _uiContentStorage,
                _serviceProvider,
                PropTransferMachineStore.Inventory,
                PropTransferMachineStore.Container,
                string.Format(UiResources.PutPropInContainerButtonTitleTemplate, _containerName));

            contextMenu.Init(selectedProp);

            _inventoryPropContextMenu = contextMenu;
        }

        private void UpdateContainer()
        {
            if (_currentContainerItems is null)
            {
                return;
            }

            foreach (var item in _currentContainerItems)
            {
                item.Control.Update();
            }
        }

        private void UpdateInventory()
        {
            if (_currentInventoryItems is null)
            {
                return;
            }

            foreach (var item in _currentInventoryItems)
            {
                item.Control.Update();
            }
        }

        private record InventoryUiItem
        {
            public InventoryUiItem(IconButton control, IProp prop, int uiIndex, Rectangle uiRect)
            {
                UiIndex = uiIndex;
                UiRect = uiRect;
                Prop = prop ?? throw new ArgumentNullException(nameof(prop));
                Control = control ?? throw new ArgumentNullException(nameof(control));
            }

            public IconButton Control { get; }
            public IProp Prop { get; }

            public int UiIndex { get; }
            public Rectangle UiRect { get; }
        }
    }
}