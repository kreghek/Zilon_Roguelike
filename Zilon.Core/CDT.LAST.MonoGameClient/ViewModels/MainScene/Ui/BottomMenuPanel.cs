using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Tactics.Behaviour;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class BottomMenuPanel
    {
        private const int SWITCHER_MODE_BUTTON_WIDTH = 16;
        private const int SWITCHER_MODE_BUTTON_HEIGHT = 32;

        private const int PANEL_WIDTH = 32 * 8;
        private const int PANEL_MARGIN = 4;

        private readonly ICombatActModule _combatActModule;
        private readonly CombatActPanel _combatActPanel;
        private readonly IconButton _combatModeSwitcherButton;

        private readonly IconButton _idleModeSwitcherButton;

        private readonly TravelPanel _travelPanel;

        private IBottomSubPanel _currentModeMenu;

        public BottomMenuPanel(
            IHumanActorTaskSource<ISectorTaskSourceContext> humanActorTaskSource,
            ICombatActModule combatActModule,
            IUiContentStorage uiContentStorage,
            IEquipmentModule equipmentModule,
            ISectorUiState sectorUiState)
        {
            _travelPanel = new TravelPanel(humanActorTaskSource, uiContentStorage);
            _combatActPanel = new CombatActPanel(combatActModule, equipmentModule, uiContentStorage, sectorUiState);

            _travelPanel.PropButtonClicked += PersonPropButton_OnClick;
            _travelPanel.StatButtonClicked += PersonStatsButton_OnClick;

            _currentModeMenu = _travelPanel;

            var idleButtonIcon = new IconData(
                uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                new Rectangle(48, 0, 16, 32)
            );

            var combatButtonIcon = new IconData(
                uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                new Rectangle(0, 32, 16, 32)
            );

            _idleModeSwitcherButton = new IconButton(uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                combatButtonIcon,
                new Rectangle(0, 0, 16, 32));
            _idleModeSwitcherButton.OnClick += IdleModeSwitcherButton_OnClick;
            _combatActModule = combatActModule;

            _combatModeSwitcherButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: idleButtonIcon,
                rect: new Rectangle(0, 0, 16, 32));
            _combatModeSwitcherButton.OnClick += CombatModeSwitcherButton_OnClick;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            _currentModeMenu.Draw(spriteBatch, graphicsDevice);

            const int COMBAT_ACT_BUTTON_SIZE = 32;
            const int BOTTOM_MARGIN = 0;
            const int MAX_COMBAT_ACT_COUNT = 8;

            var panelX = (graphicsDevice.Viewport.Width - PANEL_WIDTH) / 2;
            var panelY = graphicsDevice.Viewport.Bounds.Bottom - COMBAT_ACT_BUTTON_SIZE - BOTTOM_MARGIN;

            if (!_combatActModule.IsCombatMode)
            {
                _combatModeSwitcherButton.Rect = new Rectangle(
                    panelX + (COMBAT_ACT_BUTTON_SIZE * MAX_COMBAT_ACT_COUNT) - PANEL_MARGIN,
                    panelY - PANEL_MARGIN,
                    SWITCHER_MODE_BUTTON_WIDTH,
                    SWITCHER_MODE_BUTTON_HEIGHT);

                _combatModeSwitcherButton.Draw(spriteBatch);
            }
            else
            {
                _idleModeSwitcherButton.Rect = new Rectangle(
                    panelX + (COMBAT_ACT_BUTTON_SIZE * MAX_COMBAT_ACT_COUNT) - PANEL_MARGIN,
                    panelY - PANEL_MARGIN,
                    SWITCHER_MODE_BUTTON_WIDTH,
                    SWITCHER_MODE_BUTTON_HEIGHT);

                _idleModeSwitcherButton.Draw(spriteBatch);
            }
        }

        public void UnsubscribeEvents()
        {
            _combatActPanel.UnsubscribeEvents();
        }

        public void Update()
        {
            _currentModeMenu.Update();

            if (!_combatActModule.IsCombatMode)
            {
                _combatModeSwitcherButton.Update();
            }
            else
            {
                _idleModeSwitcherButton.Update();
            }
        }

        private void CombatModeSwitcherButton_OnClick(object? sender, EventArgs e)
        {
            _currentModeMenu = _combatActPanel;
            _combatActModule.IsCombatMode = true;
        }

        private void IdleModeSwitcherButton_OnClick(object? sender, EventArgs e)
        {
            _currentModeMenu = _travelPanel;
            _combatActModule.IsCombatMode = false;
        }

        private void PersonPropButton_OnClick(object? sender, EventArgs e)
        {
            PropButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void PersonStatsButton_OnClick(object? sender, EventArgs e)
        {
            StatButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? PropButtonClicked;
        public event EventHandler? StatButtonClicked;
    }
}