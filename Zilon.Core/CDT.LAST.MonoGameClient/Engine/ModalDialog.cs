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
        private readonly Button _closeButton;
        private readonly Rectangle _dialogRect;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Texture2D _shadowTexture;

        private readonly Texture2D _backgroundTexture;

        public ModalDialog(Texture2D backgroundTexture, Texture2D shadowTexture, Texture2D buttonTexture,
            SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _shadowTexture = shadowTexture;
            _graphicsDevice = graphicsDevice;
            _backgroundTexture = backgroundTexture;

            var modalWidth = 400;
            var modalHeight = 300;

            _dialogRect = new Rectangle(
                graphicsDevice.Viewport.Width / 2 - modalWidth / 2,
                graphicsDevice.Viewport.Height / 2 - modalHeight / 2,
                modalWidth,
                modalHeight);

            _closeButton = new Button("X", buttonTexture, font,
                new Rectangle(_dialogRect.Right - CLOSE_BUTTON_SIZE - CLOSE_BUTTON_PADDING,
                    _dialogRect.Top + CLOSE_BUTTON_PADDING, CLOSE_BUTTON_SIZE, CLOSE_BUTTON_SIZE));
            _closeButton.OnClick += CloseButton_OnClick;
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
            spriteBatch.Draw(_backgroundTexture, _dialogRect, Color.White);

            _closeButton.Draw(spriteBatch);
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

            _closeButton.Update();
        }

        private void CloseButton_OnClick(object? sender, EventArgs e)
        {
            Close();
        }
    }
}