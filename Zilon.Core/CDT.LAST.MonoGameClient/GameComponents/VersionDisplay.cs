using System;
using System.IO;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.GameComponents
{
    internal class VersionDisplay : DrawableGameComponent
    {
        private readonly SpriteFont _font;
        private readonly SpriteBatch _spriteBatch;
        private readonly string? _version;

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

            const int BORDER = 2;
            const int SHADOW_OFFSET = 1;
            var position = new Vector2(Game.GraphicsDevice.Viewport.Bounds.Right - size.X - BORDER,
                    Game.GraphicsDevice.Viewport.Bounds.Bottom - size.Y - BORDER);
            var shadowOffset = new Vector2(SHADOW_OFFSET, SHADOW_OFFSET);
            var shadowPosition = position + shadowOffset;

            _spriteBatch.DrawString(
                _font,
                _version,
                shadowPosition,
                LastColors.DarkGray);
            _spriteBatch.DrawString(
                _font,
                _version,
                position,
                LastColors.LightWhite);
            _spriteBatch.End();
        }
    }
}