using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.GameComponents
{
    internal class VersionDisplay : DrawableGameComponent
    {
        private readonly string _version = "ver.2.5.8-gdw21.4";
        private SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;

        public VersionDisplay(Game game, SpriteBatch spriteBatch, SpriteFont font) : base(game)
        {
            _spriteBatch = spriteBatch;
            _font = font;
        }

        public override void Draw(GameTime gameTime)
        {
            var size = _font.MeasureString(_version);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(
                _font,
                _version,
                new Vector2(Game.GraphicsDevice.Viewport.Bounds.Right - size.X - 1 - 1,
                Game.GraphicsDevice.Viewport.Bounds.Bottom - size.Y - 1),
                Color.Black);
            _spriteBatch.DrawString(
                _font,
                _version,
                new Vector2(Game.GraphicsDevice.Viewport.Bounds.Right - size.X - 1,
                Game.GraphicsDevice.Viewport.Bounds.Bottom - size.Y),
                Color.White);
            _spriteBatch.End();
        }
    }
}