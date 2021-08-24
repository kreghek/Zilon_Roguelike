using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.GameComponents
{
    internal class VersionDisplay : DrawableGameComponent
    {
        private readonly SpriteFont _font;
        private readonly SpriteBatch _spriteBatch;
        private readonly string _version;

        public VersionDisplay(Game game, SpriteBatch spriteBatch, SpriteFont font) : base(game)
        {
            _spriteBatch = spriteBatch;
            _font = font;

            var binPath = AppContext.BaseDirectory;

            if (string.IsNullOrWhiteSpace(binPath))
            {
                throw new InvalidOperationException("Path to bin directiory is null.");
            }

            var versionFile = Path.Combine(binPath, "version.txt");

            if (File.Exists(versionFile))
            {
                _version = File.ReadAllText(versionFile);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (string.IsNullOrWhiteSpace(_version))
            {
                return;
            }

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