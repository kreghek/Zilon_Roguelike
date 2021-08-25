using System;

using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    public abstract class ModalDialogBase
    {
        private const int CLOSE_BUTTON_SIZE = 16;
        private const int CLOSE_BUTTON_PADDING = 3;

        private const int MODAL_WIDTH = 400;
        private const int MODAL_HEIGHT = 300;
        private const int MODAL_CONTENT_MARGIN = 9;
        private const int MODAL_HEADER_HEIGHT = 15;
        private readonly Texture2D _backgroundBottomTexture;

        private readonly Texture2D _backgroundTopTexture;
        private readonly TextButton _closeButton;
        private readonly Rectangle _dialogRect;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Texture2D _shadowTexture;

        protected ModalDialogBase(IUiContentStorage uiContentStorage, GraphicsDevice graphicsDevice)
        {
            _shadowTexture = uiContentStorage.GetModalShadowTexture();
            _graphicsDevice = graphicsDevice;
            _backgroundTopTexture = uiContentStorage.GetModalTopTextures()[0];
            _backgroundBottomTexture = uiContentStorage.GetModalBottomTextures()[0];

            _dialogRect = new Rectangle(
                (graphicsDevice.Viewport.Width / 2) - (MODAL_WIDTH / 2),
                (graphicsDevice.Viewport.Height / 2) - (MODAL_HEIGHT / 2),
                MODAL_WIDTH,
                MODAL_HEIGHT);

            _closeButton = new TextButton("X", uiContentStorage.GetButtonTexture(), uiContentStorage.GetButtonFont(),
                new Rectangle(_dialogRect.Right - CLOSE_BUTTON_SIZE - CLOSE_BUTTON_PADDING,
                    _dialogRect.Top + CLOSE_BUTTON_PADDING, CLOSE_BUTTON_SIZE, CLOSE_BUTTON_SIZE));
            _closeButton.OnClick += CloseButton_OnClick;

            ContentRect = new Rectangle(
                _dialogRect.Left + MODAL_CONTENT_MARGIN,
                _dialogRect.Top + MODAL_CONTENT_MARGIN + MODAL_HEADER_HEIGHT,
                _dialogRect.Width - MODAL_CONTENT_MARGIN * 2,
                _dialogRect.Height - MODAL_CONTENT_MARGIN * 2 - MODAL_HEADER_HEIGHT);
        }

        public bool IsVisible { get; private set; }

        protected Rectangle ContentRect { get; set; }

        public void Close()
        {
            IsVisible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_shadowTexture,
                new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height),
                Color.White * 0.5f);

            const int MODAL_HALF_HEIGHT = MODAL_HEIGHT / 2;
            var topRect = new Rectangle(_dialogRect.Location, new Point(MODAL_WIDTH, MODAL_HALF_HEIGHT));
            var bottomRect = new Rectangle(new Point(_dialogRect.Left, _dialogRect.Top + MODAL_HALF_HEIGHT),
                new Point(MODAL_WIDTH, MODAL_HALF_HEIGHT));
            spriteBatch.Draw(_backgroundTopTexture, topRect, Color.White);
            spriteBatch.Draw(_backgroundBottomTexture, bottomRect, Color.White);

            DrawContent(spriteBatch);

            _closeButton.Draw(spriteBatch);
        }

        public void Show()
        {
            InitContent();
            IsVisible = true;
        }

        public void Update()
        {
            // Poll for current keyboard state
            var state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.C))
            {
                Close();
            }

            UpdateContent();

            _closeButton.Update();
        }

        protected virtual void DrawContent(SpriteBatch spriteBatch)
        {
        }

        protected virtual void InitContent()
        {
        }

        protected virtual void UpdateContent()
        {
        }

        private void CloseButton_OnClick(object? sender, EventArgs e)
        {
            Close();
        }
    }
}