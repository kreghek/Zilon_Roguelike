﻿using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics.Behaviour;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class BottomMenuPanel
    {
        private const int SWITCHER_MODE_BUTTON_WIDTH = 16;
        private const int SWITCHER_MODE_BUTTON_HEIGHT = 32;

        private const int SLOT_SIZE = 32;
        private const int SLOL_MAX_COUNT = 8;

        private const int PANEL_MARGIN = 4;
        private const int PANEL_WIDTH = (SLOT_SIZE * SLOL_MAX_COUNT) + SWITCHER_MODE_BUTTON_WIDTH + PANEL_MARGIN;
        private const int PANEL_HEIGHT = SLOT_SIZE + (PANEL_MARGIN * 2);

        private readonly ICombatActModule _combatActModule;
        private readonly CombatActPanel _combatActPanel;
        private readonly IconButton _combatModeSwitcherButton;

        private readonly IconButton _idleModeSwitcherButton;
        private readonly IPlayerEventLogService _logService;
        private readonly ISectorUiState _sectorUiState;

        private readonly TravelPanel _travelPanel;
        private readonly IUiContentStorage _uiContentStorage;

        private IBottomSubPanel _currentModeMenu;

        private KeyboardState? _lastKeyboard;
        private Rectangle _storedPanelRect;

        public BottomMenuPanel(
            IHumanActorTaskSource<ISectorTaskSourceContext> humanActorTaskSource,
            ICombatActModule combatActModule,
            IUiContentStorage uiContentStorage,
            IEquipmentModule equipmentModule,
            ISectorUiState sectorUiState,
            ICommandPool commandPool,
            ServiceProviderCommandFactory commandFactory,
            ICommandLoopContext commandLoopContext,
            IPlayerEventLogService logService)
        {
            _travelPanel = new TravelPanel(humanActorTaskSource, uiContentStorage, commandPool, commandFactory,
                commandLoopContext);
            _combatActPanel = new CombatActPanel(combatActModule, equipmentModule, uiContentStorage, sectorUiState);

            _travelPanel.PropButtonClicked += PersonPropButton_OnClick;
            _travelPanel.StatButtonClicked += PersonStatsButton_OnClick;
            _travelPanel.TraitsButtonClicked += PersonTraitsButton_OnClick;
            _travelPanel.FastDeathButtonClicked += FastDeathButtonClicked;

            _currentModeMenu = _travelPanel;

            var combatButtonIcon = new IconData(
                uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                new Rectangle(48, 0, SWITCHER_MODE_BUTTON_WIDTH, SWITCHER_MODE_BUTTON_HEIGHT)
            );

            var idleButtonIcon = new IconData(
                uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                new Rectangle(0, 32, SWITCHER_MODE_BUTTON_WIDTH, SWITCHER_MODE_BUTTON_HEIGHT)
            );

            _idleModeSwitcherButton = new IconButton(
                uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                idleButtonIcon,
                new Rectangle(0, 0, SWITCHER_MODE_BUTTON_WIDTH, SWITCHER_MODE_BUTTON_HEIGHT));
            _idleModeSwitcherButton.OnClick += IdleModeSwitcherButton_OnClick;
            _combatActModule = combatActModule;
            _uiContentStorage = uiContentStorage;
            _sectorUiState = sectorUiState;
            _logService = logService;
            _combatModeSwitcherButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: combatButtonIcon,
                rect: new Rectangle(0, 0, SWITCHER_MODE_BUTTON_WIDTH, SWITCHER_MODE_BUTTON_HEIGHT));
            _combatModeSwitcherButton.OnClick += CombatModeSwitcherButton_OnClick;
        }

        public static bool MouseIsOver { get; private set; }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var panelRect = GetPanelRectangle(graphicsDevice);

            _storedPanelRect = panelRect;

            DrawBackground(spriteBatch, panelRect);

            var contentRect = new Rectangle(
                panelRect.Left + PANEL_MARGIN,
                panelRect.Top + PANEL_MARGIN,
                panelRect.Width - (PANEL_MARGIN * 2),
                panelRect.Height - (PANEL_MARGIN * 2));

            _currentModeMenu.Draw(spriteBatch, contentRect);

            var buttonRectangle = new Rectangle(
                contentRect.Right - SWITCHER_MODE_BUTTON_WIDTH,
                contentRect.Top,
                SWITCHER_MODE_BUTTON_WIDTH,
                SWITCHER_MODE_BUTTON_HEIGHT);

            var activeButton = GetActiveSwitcherButton();

            activeButton.Rect = buttonRectangle;
            activeButton.Draw(spriteBatch);
        }

        public void UnsubscribeEvents()
        {
            _combatActPanel.UnsubscribeEvents();
        }

        public void Update()
        {
            DetectMouseIsOver();

            _currentModeMenu.Update();

            var activeSwitcherButton = GetActiveSwitcherButton();
            activeSwitcherButton.Update();

            HandleHotkeys();
        }

        private void CombatModeSwitcherButton_OnClick(object? sender, EventArgs e)
        {
            _currentModeMenu = _combatActPanel;
            _combatActModule.IsCombatMode = true;
        }

        private void DetectMouseIsOver()
        {
            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            MouseIsOver = _storedPanelRect.Intersects(mouseRect);
        }

        private void DrawBackground(SpriteBatch spriteBatch, Rectangle panelRect)
        {
            spriteBatch.Draw(_uiContentStorage.GetBottomPanelBackground(),
                panelRect,
                Color.White);
        }

        private void FastDeathButtonClicked(object? sender, EventArgs e)
        {
            var endOfLifeEvent = new EndOfLifeEvent();
            _logService.Log(endOfLifeEvent);

            var survivalModule = _sectorUiState.ActiveActor.Actor.Person.GetModule<ISurvivalModule>();
            survivalModule.SetStatForce(SurvivalStatType.Health, 0);
        }

        private IconButton GetActiveSwitcherButton()
        {
            return _combatActModule.IsCombatMode ? _idleModeSwitcherButton : _combatModeSwitcherButton;
        }

        private static Rectangle GetPanelRectangle(GraphicsDevice graphicsDevice)
        {
            var panelX = (graphicsDevice.Viewport.Width - PANEL_WIDTH) / 2;

            return new Rectangle(panelX, graphicsDevice.Viewport.Height - PANEL_HEIGHT, PANEL_WIDTH, PANEL_HEIGHT);
        }

        private void HandleHotkeys()
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyUp(Keys.Space) && _lastKeyboard?.IsKeyDown(Keys.Space) == true)
            {
                _travelPanel._idleButton.Click();
            }

            _lastKeyboard = keyboardState;
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

        private void PersonTraitsButton_OnClick(object? sender, EventArgs e)
        {
            TraitsButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? PropButtonClicked;
        public event EventHandler? StatButtonClicked;
        public event EventHandler? TraitsButtonClicked;
    }
}