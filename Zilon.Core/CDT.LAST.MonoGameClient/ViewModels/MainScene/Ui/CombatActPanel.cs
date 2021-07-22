using System;
using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class CombatActPanel : IBottomSubPanel
    {
        private const int MAX_COMBAT_ACT_COUNT = 8;
        private const int COMBAT_ACT_BUTTON_SIZE = 32;

        private readonly CombatActButtonGroup _buttonGroup;
        private readonly IList<CombatActButton> _buttons;
        private readonly ICombatActModule _combatActModule;
        private readonly IEquipmentModule _equipmentModule;
        private readonly ISectorUiState _sectorUiState;
        private readonly IUiContentStorage _uiContentStorage;

        public CombatActPanel(ICombatActModule combatActModule, IEquipmentModule equipmentModule,
            IUiContentStorage uiContentStorage, ISectorUiState sectorUiState)
        {
            _combatActModule = combatActModule;
            _equipmentModule = equipmentModule;
            _uiContentStorage = uiContentStorage;
            _sectorUiState = sectorUiState;

            _buttons = new List<CombatActButton>();

            _buttonGroup = new CombatActButtonGroup();

            Initialize(_buttons);

            _equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
        }

        private void DrawButtonHotkey(int actIndex, ButtonBase button, SpriteBatch spriteBatch)
        {
            var spriteFont = _uiContentStorage.GetAuxTextFont();

            var text = (actIndex + 1).ToString();
            var stringSize = spriteFont.MeasureString(text);

            var textX = button.Rect.Center.X;
            var textY = button.Rect.Top - stringSize.Y;

            spriteBatch.DrawString(spriteFont, text, new Vector2(textX, textY), Color.White);
        }

        private void EquipmentModule_EquipmentChanged(object? sender, EquipmentChangedEventArgs e)
        {
            _buttons.Clear();
            Initialize(_buttons);
        }

        private static int? GetNumberByKeyboardState(KeyboardState keyboardState)
        {
            var pressedKeys = keyboardState.GetPressedKeys();
            if (pressedKeys is null || !pressedKeys.Any())
            {
                return null;
            }

            var firstPressedKeyCode = (int)pressedKeys[0];
            const int FIRST_CODE = 49;
            const int LAST_CODE = 57;

            if (firstPressedKeyCode >= FIRST_CODE && firstPressedKeyCode <= LAST_CODE)
            {
                return (firstPressedKeyCode - FIRST_CODE) + 1;
            }

            return null;
        }

        private void HandleHotkeys()
        {
            var keyboardState = Keyboard.GetState();
            var buttonNumber = GetNumberByKeyboardState(keyboardState);

            if (buttonNumber is not null && buttonNumber <= _buttons.Count)
            {
                var buttonIndex = buttonNumber.Value - 1;
                var pressedButton = _buttons[buttonIndex];

                if (_buttonGroup.Selected != pressedButton)
                {
                    pressedButton.Click();
                }
            }
        }

        private void Initialize(IList<CombatActButton> buttons)
        {
            var acts = _combatActModule.GetCurrentCombatActs();
            var actsOrdered = acts.OrderBy(x => x.Scheme?.Sid).Take(MAX_COMBAT_ACT_COUNT).ToArray();
            foreach (var act in actsOrdered)
            {
                var tags = act.Scheme?.Stats?.Tags?.Where(x => x != null)?.Select(x => x!)?.ToArray() ??
                           Array.Empty<string>();
                var button = new CombatActButton(_uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetCombatActIconTexture(act.Scheme?.Sid, tags),
                    selectedMarkerTexture: _uiContentStorage.GetSelectedButtonMarkerTexture(),
                    _buttonGroup,
                    act,
                    new Rectangle(0, 0, COMBAT_ACT_BUTTON_SIZE, COMBAT_ACT_BUTTON_SIZE));

                button.OnClick += (s, e) =>
                {
                    _sectorUiState.TacticalAct = act;
                    _buttonGroup.Selected = (CombatActButton?)s;
                };

                if (act == _sectorUiState.TacticalAct)
                {
                    _buttonGroup.Selected = button;
                }

                buttons.Add(button);
            }
        }

        public void UnsubscribeEvents()
        {
            _equipmentModule.EquipmentChanged -= EquipmentModule_EquipmentChanged;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            for (var actIndex = 0; actIndex < _buttons.Count; actIndex++)
            {
                var button = _buttons[actIndex];

                var buttonStackOffsetX = actIndex * COMBAT_ACT_BUTTON_SIZE;
                var buttonRect = new Rectangle(
                    buttonStackOffsetX + contentRect.Left,
                    contentRect.Top,
                    COMBAT_ACT_BUTTON_SIZE,
                    COMBAT_ACT_BUTTON_SIZE);

                button.Rect = buttonRect;

                button.Draw(spriteBatch);

#if SHOW_NUMS
                spriteBatch.DrawString(_uiContentStorage.GetAuxTextFont(),
                    GetRollAsString(button.CombatAct.Efficient), new Vector2(buttonRect.Left, buttonRect.Top), Color.White);
#endif

                DrawButtonHotkey(actIndex, button, spriteBatch);
            }
        }

        private static string GetRollAsString(Roll roll)
        {
            return $"{roll.Count}D{roll.Dice} +{roll.Modifiers?.ResultBuff ?? 0}";
        }

        public void Update()
        {
            HandleHotkeys();

            _buttonGroup.Selected = null;
            foreach (var button in _buttons)
            {
                button.Update();

                if (button.CombatAct == _sectorUiState.TacticalAct)
                {
                    _buttonGroup.Selected = button;
                }
            }
        }
    }
}