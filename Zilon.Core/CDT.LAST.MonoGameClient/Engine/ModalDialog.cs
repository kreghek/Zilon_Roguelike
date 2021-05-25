using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    public class ModalDialog
    {
        private readonly SpriteFont _font;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Rectangle _dialogRect;
        private readonly Button _closeButton;
        private readonly Texture2D _shadowTexture;

        public ModalDialog(string title, Texture2D backgroundTexture, Texture2D shadowTexture, SpriteFont font,
            GraphicsDevice graphicsDevice)
        {
            _shadowTexture = shadowTexture;
            _font = font;
            _graphicsDevice = graphicsDevice;
            Title = title;
            BackgroundTexture = backgroundTexture;

            var modalWidth = 400;
            var modalHeight = 300;

            _dialogRect = new Rectangle(
                graphicsDevice.Viewport.Width / 2 - modalWidth / 2,
                graphicsDevice.Viewport.Height / 2 - modalHeight / 2,
                modalWidth,
                modalHeight);

            _closeButton = new Button("X", BackgroundTexture, font, new Rectangle(_dialogRect.Right - 16, _dialogRect.Top, 16, 16));
            _closeButton.OnClick += CloseButton_OnClick;
        }

        private void CloseButton_OnClick(object? sender, System.EventArgs e)
        {
            Close();
        }

        public Texture2D BackgroundTexture { get; }
        public bool IsVisible { get; set; }
        public string Title { get; set; }

        public void Close()
        {
            IsVisible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_shadowTexture,
                new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height),
                Color.White * 0.5f);
            spriteBatch.Draw(BackgroundTexture, _dialogRect, Color.White);

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
    }
}