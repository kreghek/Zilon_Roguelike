using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    public class ModalDialog
    {
        private const int CLOSE_BUTTON_SIZE = 16;
        private const int CLOSE_BUTTON_PADDING = 3;

        private const int MODAL_WIDTH = 400;
        private const int MODAL_HEIGHT = 300;
        private readonly Texture2D _backgroundBottomTexture;

        private readonly Texture2D _backgroundTopTexture;
        private readonly Button _closeButton;
        private readonly Rectangle _dialogRect;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Texture2D _shadowTexture;

        protected Rectangle ContentRect { get; set; }

        public ModalDialog(Texture2D backgroundTopTexture, Texture2D backgroundBottomTexture, Texture2D shadowTexture,
            Texture2D buttonTexture, SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _shadowTexture = shadowTexture;
            _graphicsDevice = graphicsDevice;
            _backgroundTopTexture = backgroundTopTexture;
            _backgroundBottomTexture = backgroundBottomTexture;

            _dialogRect = new Rectangle(
                (graphicsDevice.Viewport.Width / 2) - (MODAL_WIDTH / 2),
                (graphicsDevice.Viewport.Height / 2) - (MODAL_HEIGHT / 2),
                MODAL_WIDTH,
                MODAL_HEIGHT);

            _closeButton = new Button("X", buttonTexture, font,
                new Rectangle(_dialogRect.Right - CLOSE_BUTTON_SIZE - CLOSE_BUTTON_PADDING,
                    _dialogRect.Top + CLOSE_BUTTON_PADDING, CLOSE_BUTTON_SIZE, CLOSE_BUTTON_SIZE));
            _closeButton.OnClick += CloseButton_OnClick;

            ContentRect = new Rectangle(_dialogRect.Left + 5, _dialogRect.Top + 25, _dialogRect.Right - 5, _dialogRect.Bottom - 5);
        }

        public bool IsVisible { get; private set; }

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

            var contentRect = new Rectangle(_dialogRect.Left + 5, _dialogRect.Top + 25, _dialogRect.Right - 5, _dialogRect.Bottom - 5);

            DrawContent(spriteBatch, contentRect);

            _closeButton.Draw(spriteBatch);
        }

        protected virtual void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
        }

        public void Show()
        {
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

        protected virtual void UpdateContent()
        {
        }

        private void CloseButton_OnClick(object? sender, EventArgs e)
        {
            Close();
        }
    }
}