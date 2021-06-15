using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Tactics.Behaviour;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class BottomMenuPanel
    {
        private readonly IconButton _autoplayModeButton;
        private readonly IconButton _personPropButton;
        private readonly IconButton _personStatsButton;
        private readonly IconButton _combatModeSwitcherButton;
        private IconButton[] _buttons;

        private readonly IHumanActorTaskSource<ISectorTaskSourceContext> _humanActorTaskSource;
        private readonly IUiContentStorage _uiContentStorage;

        private bool _autoplayHintIsShown;
        private string _autoplayModeButtonTitle;

        public BottomMenuPanel(IHumanActorTaskSource<ISectorTaskSourceContext> humanActorTaskSource, IUiContentStorage uiContentStorage)
        {
            _humanActorTaskSource = humanActorTaskSource;
            _uiContentStorage = uiContentStorage;
            _autoplayModeButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(0, 0, 16, 32)
                ),
                rect: new Rectangle(0, 0, 16, 32)
            );
            _autoplayModeButton.OnClick += AutoplayModeButton_OnClick;
            _autoplayModeButtonTitle = string.Format(UiResources.SwitchAutomodeButtonTitle,
                UiResources.SwitchAutomodeButtonOffTitle);

            _personPropButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(16, 0, 16, 32)
                ),
                rect: new Rectangle(0, 0, 16, 32));
            _personPropButton.OnClick += PersonEquipmentButton_OnClick;

            _personStatsButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(0, 32, 16, 32)
                ),
                rect: new Rectangle(0, 0, 16, 32));
            _personStatsButton.OnClick += PersonStatsButton_OnClick;

            _combatModeSwitcherButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(0, 32, 16, 32)
                ),
                rect: new Rectangle(0, 0, 16, 32));
            _combatModeSwitcherButton.OnClick += CombatModeSwitcherButton_OnClick;

            _buttons = new[] {
                _autoplayModeButton,
                _personPropButton,
                _personStatsButton,
                _combatModeSwitcherButton
            };
        }

        private void CombatModeSwitcherButton_OnClick(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Update(GraphicsDevice graphicsDevice)
        {
            _autoplayModeButton.Update();

            _personPropButton.Update();
            _personStatsButton.Update();

            DetectAutoplayHint(graphicsDevice);
        }

        private const int BUTTON_WIDTH = 16;
        private const int BUTTON_HEIGHT = 32;

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var halfOfScreenX = graphicsDevice.Viewport.Width / 2;
            var bottomOfScreenY = graphicsDevice.Viewport.Height;

            for (var i = 0; i < _buttons.Length; i++)
            {
                var button = _buttons[i];
                button.Rect = new Rectangle(halfOfScreenX + BUTTON_WIDTH * i, bottomOfScreenY - BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT);
            }

            _autoplayModeButton.Draw(spriteBatch);
            if (_autoplayHintIsShown)
            {
                var titleTextSizeVector = _uiContentStorage.GetHintTitleFont().MeasureString(_autoplayModeButtonTitle);

                const int HINT_TEXT_SPACING = 8;

                var autoplayButtonRect = new Rectangle(halfOfScreenX - 16, bottomOfScreenY - 32, 16, 32);

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

            _personPropButton.Draw(spriteBatch);
            _personStatsButton.Draw(spriteBatch);
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

        private void DetectAutoplayHint(GraphicsDevice graphicsDevice)
        {
            var halfOfScreenX = graphicsDevice.Viewport.Width / 2;
            var bottomOfScreenY = graphicsDevice.Viewport.Height;

            var autoplayButtonRect = new Rectangle(halfOfScreenX - 16, bottomOfScreenY - 32, 16, 32);

            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            _autoplayHintIsShown = autoplayButtonRect.Intersects(mouseRect);
        }

        private void PersonEquipmentButton_OnClick(object? sender, EventArgs e)
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