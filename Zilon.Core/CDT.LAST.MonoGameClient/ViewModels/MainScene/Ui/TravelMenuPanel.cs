using System;
using System.Diagnostics;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Commands;
using Zilon.Core.Tactics.Behaviour;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class TravelPanel : IBottomSubPanel
    {
        private const int BUTTON_WIDTH = 16;
        private const int BUTTON_HEIGHT = 32;
        private const int HINT_TEXT_SPACING = 8;

        private readonly IconButton _autoplayModeButton;
        private readonly IconButton[] _buttons;

        private readonly IHumanActorTaskSource<ISectorTaskSourceContext> _humanActorTaskSource;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly ICommandPool _commandPool;
        private readonly ServiceProviderCommandFactory _commandFactory;
        private bool _autoplayHintIsShown;
        private string _autoplayModeButtonTitle;

        public TravelPanel(IHumanActorTaskSource<ISectorTaskSourceContext> humanActorTaskSource,
            IUiContentStorage uiContentStorage,
            ICommandPool commandPool,
            ServiceProviderCommandFactory commandFactory)
        {
            _humanActorTaskSource = humanActorTaskSource;
            _uiContentStorage = uiContentStorage;
            _commandPool = commandPool;
            _commandFactory = commandFactory;
            _autoplayModeButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT)
                ),
                rect: new Rectangle(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT)
            );
            _autoplayModeButton.OnClick += AutoplayModeButton_OnClick;
            _autoplayModeButtonTitle = string.Format(UiResources.SwitchAutomodeButtonTitle,
                UiResources.SwitchAutomodeButtonOffTitle);

            var personPropButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(16, 0, BUTTON_WIDTH, BUTTON_HEIGHT)
                ),
                rect: new Rectangle(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT));
            personPropButton.OnClick += PersonEquipmentButton_OnClick;

            var personStatsButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(32, 0, BUTTON_WIDTH, BUTTON_HEIGHT)
                ),
                rect: new Rectangle(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT));
            personStatsButton.OnClick += PersonStatsButton_OnClick;

            var gameSpeedButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(16, 32, BUTTON_WIDTH, BUTTON_HEIGHT)
                ),
                rect: new Rectangle(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT));
            gameSpeedButton.OnClick += GameSpeedButton_OnClick;

            var idleButton = new IconButton(texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(32, 32, BUTTON_WIDTH, BUTTON_HEIGHT)
                ),
                rect: new Rectangle(0, 0, BUTTON_WIDTH, BUTTON_HEIGHT));

            idleButton.OnClick += IdleButton_OnClick;

            #if DEBUG
            _buttons = new[]
            {
                _autoplayModeButton,
                personPropButton,
                personStatsButton,
                gameSpeedButton
            }; 
            #else
            _buttons = new[]
            {
                personPropButton,
                personStatsButton,
                idleButton
            };
            #endif
        }

        private void IdleButton_OnClick(object? sender, EventArgs e)
        {
            var idleCommand = _commandFactory.GetCommand<IdleCommand>();
            _commandPool.Push(idleCommand);
        }

        private void AutoplayModeButton_OnClick(object? sender, EventArgs e)
        {
            var humanTaskSource = _humanActorTaskSource;
            if (humanTaskSource is IActorTaskControlSwitcher controlSwitcher)
            {
                switch (controlSwitcher.CurrentControl)
                {
                    case ActorTaskSourceControl.Human:
                        controlSwitcher.Switch(ActorTaskSourceControl.Bot);
                        _autoplayModeButtonTitle = string.Format(UiResources.SwitchAutomodeButtonTitle,
                            UiResources.SwitchAutomodeButtonOnTitle);
                        break;

                    case ActorTaskSourceControl.Bot:
                        controlSwitcher.Switch(ActorTaskSourceControl.Human);
                        _autoplayModeButtonTitle = string.Format(UiResources.SwitchAutomodeButtonTitle,
                            UiResources.SwitchAutomodeButtonOffTitle);
                        break;

                    default:
                        throw new InvalidOperationException(
                            "Unknown actor task control {controlSwitcher.CurrentControl}.");
                }
            }
        }

        private void DetectAutoplayHint()
        {
            var autoplayButtonRect = _autoplayModeButton.Rect;

            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            _autoplayHintIsShown = autoplayButtonRect.Intersects(mouseRect);
        }

        private void GameSpeedButton_OnClick(object? sender, EventArgs e)
        {
            if (GameState.GameSpeed == 1)
            {
                GameState.GameSpeed = 2;
            }
            else if (GameState.GameSpeed == 2)
            {
                GameState.GameSpeed = 4;
            }
            else if (GameState.GameSpeed == 4)
            {
                GameState.GameSpeed = 1;
            }
            else
            {
                Debug.Fail("Unknown game state.");
                GameState.GameSpeed = 1;
            }
        }

        private void PersonEquipmentButton_OnClick(object? sender, EventArgs e)
        {
            PropButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void PersonStatsButton_OnClick(object? sender, EventArgs e)
        {
            StatButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            for (var buttonIndex = 0; buttonIndex < _buttons.Length; buttonIndex++)
            {
                var button = _buttons[buttonIndex];
                var buttonOffsetX = BUTTON_WIDTH * buttonIndex;
                button.Rect = new Rectangle(
                    contentRect.Left + buttonOffsetX,
                    contentRect.Top,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT);

                button.Draw(spriteBatch);
            }

            if (_autoplayHintIsShown)
            {
                var titleTextSizeVector = _uiContentStorage.GetHintTitleFont().MeasureString(_autoplayModeButtonTitle);

                var autoplayButtonRect = _autoplayModeButton.Rect;

                var hintRectangle = new Rectangle(
                    autoplayButtonRect.Left,
                    autoplayButtonRect.Top - (int)titleTextSizeVector.Y - (HINT_TEXT_SPACING * 2),
                    (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                    (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

                spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

                spriteBatch.DrawString(_uiContentStorage.GetHintTitleFont(),
                    _autoplayModeButtonTitle,
                    new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                    Color.Wheat);
            }
        }

        public void Update()
        {
            foreach (var button in _buttons)
            {
                button.Update();
            }

            DetectAutoplayHint();
        }

        public void UnsubscribeEvents()
        {
            // This panel has no event handler of the globe objects.
            // So there is noting to unsubscribe now.
        }

        public event EventHandler? PropButtonClicked;
        public event EventHandler? StatButtonClicked;
    }
}