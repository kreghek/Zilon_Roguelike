using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    public class ModalDialog
    {
        private readonly Texture2D _shadowTexture;
        private readonly SpriteFont _font;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Rectangle _rect;

        public ModalDialog(string title, Texture2D backgroundTexture, Texture2D shadowTexture, SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _shadowTexture = shadowTexture;
            _font = font;
            _graphicsDevice = graphicsDevice;
            Title = title;
            BackgroundTexture = backgroundTexture;

            var modalWidth = 400;
            var modalHeight = 300;

            _rect = new Rectangle(
                graphicsDevice.Viewport.Width / 2 - modalWidth / 2,
                graphicsDevice.Viewport.Height / 2 - modalHeight / 2,
                modalWidth,
                modalHeight);
        }

        public Texture2D BackgroundTexture { get; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }

        public void Show()
        {
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_shadowTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), new Color(1f, 1f, 1f, 0.5f));
            spriteBatch.Draw(BackgroundTexture, _rect, Color.White);
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
        }
    }
}