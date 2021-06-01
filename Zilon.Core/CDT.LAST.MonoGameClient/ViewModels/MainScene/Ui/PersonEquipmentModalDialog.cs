using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.PersonModules;
using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class PersonEquipmentModalDialog : ModalDialog
    {
        private const int ICON_SIZE = 32;
        private const int ICON_SPACING = 2;

        private readonly EquipmentUiItem[] _currentEquipmentItems;
        private readonly IEquipmentModule _equipmentModule;

        private readonly IUiContentStorage _uiContentStorage;

        private EquipmentUiItem? _selectedEquipmentItem;

        public PersonEquipmentModalDialog(
            IUiContentStorage uiContentStorage,
            GraphicsDevice graphicsDevice,
            IEquipmentModule equipmentModule) : base(uiContentStorage, graphicsDevice)
        {
            _uiContentStorage = uiContentStorage;
            _equipmentModule = equipmentModule;

            var currentEquipmentItemList = new List<EquipmentUiItem>();
            foreach (var equipment in _equipmentModule)
            {
                if (equipment is null)
                {
                    continue;
                }

                var lastIndex = currentEquipmentItemList.Count;
                var buttonRect = new Rectangle((lastIndex * ICON_SIZE + ICON_SPACING) + ContentRect.Left,
                    ContentRect.Top, ICON_SIZE, ICON_SIZE);

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
                    new Rectangle(0, 14, 32, 32));

                var uiItem = new EquipmentUiItem
                { Control = equipmentButton, Equipment = equipment, UiRect = buttonRect, UiIndex = lastIndex };

                currentEquipmentItemList.Add(uiItem);
            }

            _currentEquipmentItems = currentEquipmentItemList.ToArray();
        }

        protected override void DrawContent(SpriteBatch spriteBatch)
        {
            foreach (var item in _currentEquipmentItems)
            {
                item.Control.Draw(spriteBatch);
            }

            DrawHintIfSelected(spriteBatch);
        }

        protected override void UpdateContent()
        {
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
            var selectedConditionIndex = _selectedEquipmentItem.UiIndex;
            var hintXPosition = selectedConditionIndex * (ICON_SIZE + ICON_SPACING) + ContentRect.Left;

            const int Y_POSITION_UNDER_ICON_SEQUENCE = ICON_SIZE + ICON_SPACING;
            const int HINT_TEXT_SPACING = 8;
            var hintRectangle = new Rectangle(hintXPosition, Y_POSITION_UNDER_ICON_SEQUENCE + ContentRect.Top,
                (int)titleTextSizeVector.X + HINT_TEXT_SPACING * 2,
                (int)titleTextSizeVector.Y + HINT_TEXT_SPACING * 2);

            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

            spriteBatch.DrawString(hintTitleFont, equipmentTitle,
                new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                Color.Wheat);
        }

        private static string? GetEquipmentTitle(Equipment equipment)
        {
            return equipment.Scheme.Name?.En ?? "<Undef>";
        }

        private record EquipmentUiItem
        {
            public EquipmentButton Control { get; init; }
            public Equipment Equipment { get; init; }
            public int UiIndex { get; init; }
            public Rectangle UiRect { get; init; }
        }
    }
}