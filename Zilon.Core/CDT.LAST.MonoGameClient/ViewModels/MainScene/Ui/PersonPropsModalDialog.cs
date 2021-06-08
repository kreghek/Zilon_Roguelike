using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using CDT.LAST.MonoGameClient.Engine;
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
        private readonly IServiceProvider _serviceProvider;

        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;
        private EquipmentUiItem[]? _currentEquipmentItems;

        private InventoryUiItem[]? _currentInventoryItems;
        private EquipmentUiItem? _hoverEquipmentItem;
        private InventoryUiItem? _hoverInventoryItem;
        private PropModalInventoryContextualMenu? _propSubmenu;

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
            DrawEquipments(spriteBatch);
            DrawInventory(spriteBatch);

            if (_propSubmenu != null)
            {
                _propSubmenu.Draw(spriteBatch);
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

            InitEquipment(person);
            InitInventory(person);
        }

        protected override void UpdateContent()
        {
            if (_propSubmenu != null)
            {
                _propSubmenu.Update();

                if (_propSubmenu.IsClosed)
                {
                    if (_propSubmenu.IsCommandUsed)
                    {
                        Close();
                    }

                    _propSubmenu = null;
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
            if (_hoverEquipmentItem is null)
            {
                return;
            }

            var equipmentTitle = GetPropTitle(_hoverEquipmentItem.Equipment);
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
            }
        }

        private void DrawInventoryHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_hoverInventoryItem is null)
            {
                return;
            }

            var InventoryTitle = GetPropTitle(_hoverInventoryItem.Prop);
            var hintTitleFont = _uiContentStorage.GetHintTitleFont();
            var titleTextSizeVector = hintTitleFont.MeasureString(InventoryTitle);

            const int HINT_TEXT_SPACING = 8;
            var hintRectangle = new Rectangle(
                _hoverInventoryItem.UiRect.Left,
                _hoverInventoryItem.UiRect.Bottom + EQUIPMENT_ITEM_SPACING,
                (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

            spriteBatch.DrawString(hintTitleFont, InventoryTitle,
                new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                Color.Wheat);
        }

        private static string? GetPropTitle(IProp prop)
        {
            var text = prop.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = prop.Scheme.Name?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = prop.Scheme.Name?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text ?? "<Undef>";
        }

        private void InitEquipment(IPerson person)
        {
            var equipmentModule = person.GetModuleSafe<IEquipmentModule>();
            if (equipmentModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must be able to use equipment to shown in this dialog.");
            }

            var currentEquipmentItemList = new List<EquipmentUiItem>();
            foreach (var equipment in equipmentModule)
            {
                if (equipment is null)
                {
                    continue;
                }

                var lastIndex = currentEquipmentItemList.Count;
                var relativeX = lastIndex * (EQUIPMENT_ITEM_SIZE + EQUIPMENT_ITEM_SPACING);
                var buttonRect = new Rectangle(
                    relativeX + ContentRect.Left,
                    ContentRect.Top,
                    EQUIPMENT_ITEM_SIZE,
                    EQUIPMENT_ITEM_SIZE);

                var sid = equipment.Scheme.Sid;
                if (string.IsNullOrEmpty(sid))
                {
                    Debug.Fail("All equipment must have symbolic identifier (SID).");
                    sid = "EmptyPropIcon";
                }

                var equipmentButton = new EquipmentButton(
                    _uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetPropIconLayers(sid),
                    buttonRect,
                    new Rectangle(0, 0, EQUIPMENT_ITEM_SIZE, EQUIPMENT_ITEM_SIZE));

                var uiItem = new EquipmentUiItem(equipmentButton, equipment, lastIndex, buttonRect);

                currentEquipmentItemList.Add(uiItem);
            }

            _currentEquipmentItems = currentEquipmentItemList.ToArray();
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
            var inventoryItems = inventoryModule.CalcActualItems();
            foreach (var prop in inventoryItems)
            {
                if (prop is null)
                {
                    continue;
                }

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
                    Debug.Fail("All equipment must have symbolic identifier (SID).");
                    sid = "EmptyPropIcon";
                }

                var propButton = new IconButton(
                    _uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetPropIconLayers(sid).First(),
                    buttonRect);
                propButton.OnClick += PropButton_OnClick;

                var uiItem = new InventoryUiItem(propButton, prop, lastIndex, buttonRect);

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

            _propSubmenu = new PropModalInventoryContextualMenu(mouseState.Position, selectedProp, equipmentModule,
                _uiContentStorage, _serviceProvider);
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
                Equipment = equipment ?? throw new ArgumentNullException(nameof(equipment));
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