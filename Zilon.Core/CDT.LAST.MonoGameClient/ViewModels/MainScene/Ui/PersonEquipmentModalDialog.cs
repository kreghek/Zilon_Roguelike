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
using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class PersonEquipmentModalDialog : ModalDialog
    {
        private const int EQUIPMENT_ITEM_SIZE = 32 + (5 * 2); // 5 is margin in button
        private const int EQUIPMENT_ITEM_SPACING = 2;

        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;
        private EquipmentUiItem[]? _currentEquipmentItems;
        private EquipmentUiItem? _selectedEquipmentItem;

        public PersonEquipmentModalDialog(
            IUiContentStorage uiContentStorage,
            GraphicsDevice graphicsDevice,
            ISectorUiState uiState) : base(uiContentStorage, graphicsDevice)
        {
            _uiContentStorage = uiContentStorage;
            _uiState = uiState;
        }

        protected override void DrawContent(SpriteBatch spriteBatch)
        {
            if (_currentEquipmentItems is null)
            {
                return;
            }

            foreach (var item in _currentEquipmentItems)
            {
                item.Control.Draw(spriteBatch);
            }

            DrawHintIfSelected(spriteBatch);
        }

        protected override void InitContent()
        {
            base.InitContent();

            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                throw new InvalidOperationException("Active person must be selected before this dialog was opened.");
            }

            var equipmentModule = person.GetModuleSafe<IEquipmentModule>();
            if (equipmentModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must be aple to use equipment to shown in this dialog.");
            }

            var currentEquipmentItemList = new List<EquipmentUiItem>();
            foreach (var equipment in equipmentModule)
            {
                if (equipment is null)
                {
                    continue;
                }

                var lastIndex = currentEquipmentItemList.Count;
                var relativeX = (lastIndex * EQUIPMENT_ITEM_SIZE) + EQUIPMENT_ITEM_SPACING;
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

        protected override void UpdateContent()
        {
            if (_currentEquipmentItems is null)
            {
                return;
            }

            foreach (var item in _currentEquipmentItems)
            {
                item.Control.Update();
            }

            var mouseState = Mouse.GetState();

            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            var effectUnderMouse = _currentEquipmentItems.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            _selectedEquipmentItem = effectUnderMouse;
        }

        private void DrawHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_selectedEquipmentItem is null)
            {
                return;
            }

            var equipmentTitle = GetEquipmentTitle(_selectedEquipmentItem.Equipment);
            var hintTitleFont = _uiContentStorage.GetHintTitleFont();
            var titleTextSizeVector = hintTitleFont.MeasureString(equipmentTitle);

            const int HINT_TEXT_SPACING = 8;
            var hintRectangle = new Rectangle(
                _selectedEquipmentItem.UiRect.Left,
                _selectedEquipmentItem.UiRect.Bottom + EQUIPMENT_ITEM_SPACING,
                (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

            spriteBatch.DrawString(hintTitleFont, equipmentTitle,
                new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                Color.Wheat);
        }

        private static string? GetEquipmentTitle(Equipment equipment)
        {
            var text = equipment.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = equipment.Scheme.Name?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = equipment.Scheme.Name?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text ?? "<Undef>";
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
    }
}