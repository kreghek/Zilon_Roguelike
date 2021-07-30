using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class PersonPropsModalDialog : ModalDialogBase
    {
        private const int EQUIPMENT_ITEM_SIZE = 32;
        private const int EQUIPMENT_ITEM_SPACING = 2;
        private const int MAX_INVENTORY_ROW_ITEMS = 8;
        private readonly IServiceProvider _serviceProvider;

        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;
        private EquipmentUiItem[]? _currentEquipmentItems;

        private InventoryUiItem[]? _currentInventoryItems;
        private EquipmentUiItem? _hoverEquipmentItem;
        private InventoryUiItem? _hoverInventoryItem;
        private PropModalInventoryContextualMenu? _propContextMenu;

        public PersonPropsModalDialog(
            IUiContentStorage uiContentStorage,
            GraphicsDevice graphicsDevice,
            ISectorUiState uiState,
            IServiceProvider serviceProvider) : base(uiContentStorage, graphicsDevice)
        {
            _uiContentStorage = uiContentStorage;
            _uiState = uiState;
            _serviceProvider = serviceProvider;
        }

        protected override void DrawContent(SpriteBatch spriteBatch)
        {
            // Separator
            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), new Rectangle(ContentRect.Center.X, ContentRect.Top, 2, ContentRect.Height), Color.White);

            DrawEquipments(spriteBatch);
            DrawInventory(spriteBatch);

            if (_propContextMenu != null)
            {
                _propContextMenu.Draw(spriteBatch);
            }
            else
            {
                DrawEquipmentHintIfSelected(spriteBatch);
                DrawInventoryHintIfSelected(spriteBatch);
            }
        }

        protected override void InitContent()
        {
            base.InitContent();

            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                throw new InvalidOperationException("Active person must be selected before this dialog was opened.");
            }

            var halfWidth = ContentRect.Width / 2;
            InitEquipment(person, new Rectangle(ContentRect.Left, ContentRect.Top, halfWidth - 2, ContentRect.Height));
            InitInventory(person, new Rectangle(ContentRect.Center.X + 2 * 2, ContentRect.Top, halfWidth - 2 * 2, ContentRect.Height));
        }

        protected override void UpdateContent()
        {
            if (_propContextMenu != null)
            {
                if (_propContextMenu.IsClosed)
                {
                    if (_propContextMenu.IsCommandUsed)
                    {
                        // We need to close modal and show actor animation.
                        Close();
                    }

                    _propContextMenu = null;
                }
                else
                {
                    _propContextMenu.Update();
                }
            }
            else
            {
                UpdateEquipment();
                UpdateInventory();

                var mouseState = Mouse.GetState();

                var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

                DetectHoverEquipment(mouseRectangle);
                DetectHoverInventory(mouseRectangle);
            }
        }

        private void DetectHoverEquipment(Rectangle mouseRectangle)
        {
            if (_currentEquipmentItems is null)
            {
                return;
            }

            var effectUnderMouse = _currentEquipmentItems.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            _hoverEquipmentItem = effectUnderMouse;
        }

        private void DetectHoverInventory(Rectangle mouseRectangle)
        {
            if (_currentInventoryItems is null)
            {
                return;
            }

            var effectUnderMouse = _currentInventoryItems.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            _hoverInventoryItem = effectUnderMouse;
        }

        private void DrawEquipmentHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_hoverEquipmentItem is null || _hoverEquipmentItem.Equipment is null)
            {
                return;
            }

            var equipmentTitle = PropHelper.GetPropTitle(_hoverEquipmentItem.Equipment);
            var hintTitleFont = _uiContentStorage.GetHintTitleFont();
            var titleTextSizeVector = hintTitleFont.MeasureString(equipmentTitle);

            const int HINT_TEXT_SPACING = 8;
            var hintRectangle = new Rectangle(
                _hoverEquipmentItem.UiRect.Left,
                _hoverEquipmentItem.UiRect.Bottom + EQUIPMENT_ITEM_SPACING,
                (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

            spriteBatch.DrawString(hintTitleFont, equipmentTitle,
                new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                Color.Wheat);
        }

        private void DrawEquipments(SpriteBatch spriteBatch)
        {
            if (_currentEquipmentItems is null)
            {
                return;
            }

            foreach (var item in _currentEquipmentItems)
            {
                item.Control.Draw(spriteBatch);

                if (item.Equipment is not null)
                {
                    var propTitle = PropHelper.GetPropTitle(item.Equipment);
                    spriteBatch.DrawString(_uiContentStorage.GetButtonFont(), propTitle, new Vector2(item.Control.Rect.Right + 2, item.Control.Rect.Top), Color.Wheat);
                }
                else
                {
                    spriteBatch.DrawString(_uiContentStorage.GetButtonFont(), UiResources.NoneEquipmentTitle, new Vector2(item.Control.Rect.Right + 2, item.Control.Rect.Top), Color.Gray);
                }
            }
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

                var propTitle = PropHelper.GetPropTitle(item.Prop);
                if (item.Prop is Resource resource)
                {
                    propTitle += $" x{resource.Count}";
                }
                spriteBatch.DrawString(_uiContentStorage.GetButtonFont(), propTitle, new Vector2(item.Control.Rect.Right + 2, item.Control.Rect.Top), Color.Wheat);
            }
        }

        private void DrawInventoryHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_hoverInventoryItem is null)
            {
                return;
            }

            var inventoryTitle = PropHelper.GetPropTitle(_hoverInventoryItem.Prop);
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

        private void InitEquipment(IPerson person, Rectangle rect)
        {
            var equipmentModule = person.GetModuleSafe<IEquipmentModule>();
            if (equipmentModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must be able to use equipment to shown in this dialog.");
            }

            var currentEquipmentItemList = new List<EquipmentUiItem>();
            var equipmentSlotList = equipmentModule.Slots.ToArray();
            for (var itemIndex = 0; itemIndex < equipmentSlotList.Length; itemIndex++)
            {
                var slot = equipmentSlotList[itemIndex];

                var sid = string.Empty;
                Equipment equipment = null;
                if (equipmentModule[itemIndex] is not null)
                {
                    equipment = equipmentModule[itemIndex];
                    sid = equipment.Scheme.Sid;
                    if (string.IsNullOrEmpty(sid))
                    {
                        Debug.Fail("All equipment must have symbolic identifier (SID).");
                        sid = "EmptyPropIcon";
                    }
                }
                else
                {
                    switch (slot.Types)
                    {
                        case Zilon.Core.Components.EquipmentSlotTypes.Head:
                            sid = "HeadSlot";
                            break;

                        case Zilon.Core.Components.EquipmentSlotTypes.Body:
                            sid = "BodySlot";
                            break;

                        case Zilon.Core.Components.EquipmentSlotTypes.Hand:
                            if (slot.IsMain)
                            {
                                sid = "RightHandSlot";
                            }
                            else
                            {
                                sid = "LeftHandSlot";
                            }
                            break;

                        case Zilon.Core.Components.EquipmentSlotTypes.Aux:
                            sid = "AuxSlot";
                            break;
                    }
                }

                var relativeY = itemIndex * (EQUIPMENT_ITEM_SIZE + EQUIPMENT_ITEM_SPACING);
                var buttonRect = new Rectangle(
                    rect.Left,
                    rect.Top + relativeY,
                    EQUIPMENT_ITEM_SIZE,
                    EQUIPMENT_ITEM_SIZE);

                var equipmentButton = new EquipmentButton(
                    _uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetPropIconLayers(sid),
                    buttonRect,
                    new Rectangle(0, 0, EQUIPMENT_ITEM_SIZE, EQUIPMENT_ITEM_SIZE));

                var uiItem = new EquipmentUiItem(equipmentButton, equipment, itemIndex, buttonRect);

                currentEquipmentItemList.Add(uiItem);
            }

            _currentEquipmentItems = currentEquipmentItemList.ToArray();
        }

        private void InitInventory(IPerson person, Rectangle rect)
        {
            var inventoryModule = person.GetModuleSafe<IInventoryModule>();
            if (inventoryModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must be able to use equipment to shown in this dialog.");
            }

            var currentInventoryItemList = new List<InventoryUiItem>();
            var inventoryItems = inventoryModule.CalcActualItems().ToArray();
            for (var itemIndex = 0; itemIndex < inventoryItems.Length; itemIndex++)
            {
                var prop = inventoryItems[itemIndex];
                if (prop is null)
                {
                    continue;
                }

                var relativeY = itemIndex * (EQUIPMENT_ITEM_SIZE + EQUIPMENT_ITEM_SPACING);
                var buttonRect = new Rectangle(
                    rect.Left,
                    rect.Top + relativeY,
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
                propButton.OnClick += PropButton_OnClick;

                var uiItem = new InventoryUiItem(propButton, prop, itemIndex, buttonRect);

                currentInventoryItemList.Add(uiItem);
            }

            _currentInventoryItems = currentInventoryItemList.ToArray();
        }

        private void PropButton_OnClick(object? sender, EventArgs e)
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

            var propContextMenu = new PropModalInventoryContextualMenu(
                mouseState.Position,
                equipmentModule,
                _uiContentStorage,
                _serviceProvider);
            propContextMenu.Init(selectedProp);
            _propContextMenu = propContextMenu;
        }

        private void UpdateEquipment()
        {
            if (_currentEquipmentItems is null)
            {
                return;
            }

            foreach (var item in _currentEquipmentItems)
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

        private record EquipmentUiItem
        {
            public EquipmentUiItem(EquipmentButton control, Equipment equipment, int uiIndex, Rectangle uiRect)
            {
                Control = control ?? throw new ArgumentNullException(nameof(control));
                Equipment = equipment;
                UiIndex = uiIndex;
                UiRect = uiRect;
            }

            public EquipmentButton Control { get; }
            public Equipment Equipment { get; }
            public int UiIndex { get; }
            public Rectangle UiRect { get; }
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