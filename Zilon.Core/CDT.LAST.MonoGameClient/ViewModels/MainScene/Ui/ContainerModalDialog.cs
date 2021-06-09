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
using Zilon.Core.Tactics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class ContainerModalDialog : ModalDialogBase
    {
        private const int EQUIPMENT_ITEM_SIZE = 32;
        private const int EQUIPMENT_ITEM_SPACING = 2;

        private readonly ISectorUiState _uiState;
        private readonly IUiContentStorage _uiContentStorage;
        private InventoryUiItem[] _currentInventoryItems;

        public ContainerModalDialog(ISectorUiState uiState, IUiContentStorage uiContentStorage, GraphicsDevice graphicsDevice) : base(
            uiContentStorage, graphicsDevice)
        {
            _uiState = uiState;
            _uiContentStorage = uiContentStorage;
        }

        internal void Init(IStaticObject container)
        {
            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                throw new InvalidOperationException("Active person must be selected before this dialog was opened.");
            }

            InitInventory(person);
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
                //propButton.OnClick += PropButton_OnClick;

                var uiItem = new InventoryUiItem(propButton, prop, lastIndex, buttonRect);

                currentInventoryItemList.Add(uiItem);
            }

            _currentInventoryItems = currentInventoryItemList.ToArray();
        }

        protected override void DrawContent(SpriteBatch spriteBatch)
        {
            DrawInventory(spriteBatch);

            if (false)
            {
                //_propSubmenu.Draw(spriteBatch);
            }
            else
            {
                DrawInventoryHintIfSelected(spriteBatch);
            }
        }

        private InventoryUiItem? _hoverInventoryItem;

        private void DrawInventoryHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_hoverInventoryItem is null)
            {
                return;
            }

            var inventoryTitle = GetPropTitle(_hoverInventoryItem.Prop);
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

        protected override void UpdateContent()
        {
            if (false)
            {
                //_propSubmenu.Update();

                //if (_propSubmenu.IsClosed)
                //{
                //    if (_propSubmenu.IsCommandUsed)
                //    {
                //        Close();
                //    }

                //    _propSubmenu = null;
                //}
            }
            else
            {
                UpdateInventory();

                var mouseState = Mouse.GetState();

                var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

                DetectHoverInventory(mouseRectangle);
            }
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