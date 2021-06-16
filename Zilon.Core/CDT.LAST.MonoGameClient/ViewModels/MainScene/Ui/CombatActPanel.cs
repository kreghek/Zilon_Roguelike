using System;
using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class CombatActPanel
    {
        private const int MAX_COMBAT_ACT_COUNT = 8;
        private readonly CombatActButtonGroup _buttonGroup;
        private readonly IList<CombatActButton> _buttons;
        private readonly ICombatActModule _combatActModule;
        private readonly IEquipmentModule _equipmentModule;
        private readonly IconButton _idleModeSwitcherButton;
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

            _idleModeSwitcherButton = new IconButton(uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                new IconData(uiContentStorage.GetSmallVerticalButtonIconsTexture(), new Rectangle(0, 32, 16, 32)),
                new Rectangle(0, 0, 16, 32));
            _idleModeSwitcherButton.OnClick += IdleModeSwitcherButton_OnClick;

            _equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            DrawBackground(spriteBatch, graphicsDevice);

            const int COMBAT_ACT_BUTTON_SIZE = 32;
            const int SWITCHER_MODE_BUTTON_WIDTH = 16;
            const int SWITCHER_MODE_BUTTON_HEIGHT = 32;
            const int BOTTOM_MARGIN = 0;

            const int PANEL_WIDTH = COMBAT_ACT_BUTTON_SIZE * MAX_COMBAT_ACT_COUNT;
            const int PANEL_MARGIN = 4;

            var panelX = (graphicsDevice.Viewport.Width - PANEL_WIDTH) / 2;
            var panelY = graphicsDevice.Viewport.Bounds.Bottom - COMBAT_ACT_BUTTON_SIZE - BOTTOM_MARGIN;

            for (var actIndex = 0; actIndex < _buttons.Count; actIndex++)
            {
                var button = _buttons[actIndex];

                var buttonStackOffsetX = actIndex * COMBAT_ACT_BUTTON_SIZE;
                var buttonRect = new Rectangle(
                    buttonStackOffsetX + panelX - PANEL_MARGIN,
                    panelY - PANEL_MARGIN,
                    COMBAT_ACT_BUTTON_SIZE,
                    COMBAT_ACT_BUTTON_SIZE);

                button.Rect = buttonRect;

                button.Draw(spriteBatch);
            }

            _idleModeSwitcherButton.Rect = new Rectangle(
                panelX + (COMBAT_ACT_BUTTON_SIZE * MAX_COMBAT_ACT_COUNT) - PANEL_MARGIN,
                panelY - PANEL_MARGIN,
                SWITCHER_MODE_BUTTON_WIDTH,
                SWITCHER_MODE_BUTTON_HEIGHT);
            _idleModeSwitcherButton.Draw(spriteBatch);
        }

        public void UnsubscribeEvents()
        {
            _equipmentModule.EquipmentChanged -= EquipmentModule_EquipmentChanged;
        }

        public void Update()
        {
            foreach (var button in _buttons)
            {
                button.Update();
            }

            _idleModeSwitcherButton.Update();
        }

        private void DrawBackground(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            const int PANEL_MARGIN = 4;
            const int PANEL_WIDTH = 32 * 8 + 16 + PANEL_MARGIN;
            const int PANEL_HEIGHT = 32 + 4 * 2;

            var panelX = (graphicsDevice.Viewport.Width - PANEL_WIDTH) / 2;

            spriteBatch.Draw(_uiContentStorage.GetBottomPanelBackground(),
                new Rectangle(panelX, graphicsDevice.Viewport.Height - PANEL_HEIGHT, PANEL_WIDTH, PANEL_HEIGHT),
                Color.White);
        }

        private void EquipmentModule_EquipmentChanged(object? sender, EquipmentChangedEventArgs e)
        {
            _buttons.Clear();
            Initialize(_buttons);
        }

        private void IdleModeSwitcherButton_OnClick(object? sender, EventArgs e)
        {
            _combatActModule.IsCombatMode = !_combatActModule.IsCombatMode;
        }

        private void Initialize(IList<CombatActButton> _buttons)
        {
            var acts = _combatActModule.CalcCombatActs();
            var actsOrdered = acts.OrderBy(x => x.Scheme?.Sid).ToArray();
            foreach (var act in actsOrdered)
            {
                const int BUTTON_SIZE = 32;
                var tags = act.Scheme?.Stats?.Tags?.Where(x => x != null)?.Select(x => x!)?.ToArray() ??
                           Array.Empty<string>();
                var button = new CombatActButton(_uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetCombatActIconTexture(act.Scheme?.Sid, tags),
                    selectedMarkerTexture: _uiContentStorage.GetSelectedButtonMarkerTexture(),
                    _buttonGroup,
                    new Rectangle(0, 0, BUTTON_SIZE, BUTTON_SIZE));

                button.OnClick += (s, e) =>
                {
                    _sectorUiState.TacticalAct = act;
                    _buttonGroup.Selected = (CombatActButton?)s;
                };

                if (act == _sectorUiState.TacticalAct)
                {
                    _buttonGroup.Selected = button;
                }

                _buttons.Add(button);
            }
        }
    }
}