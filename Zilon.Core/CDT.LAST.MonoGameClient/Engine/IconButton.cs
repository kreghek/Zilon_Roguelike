using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal class IconButton : ButtonBase
    {
        private readonly Texture2D _icon;
        private readonly Rectangle? _iconRect;

        public IconButton(Texture2D texture, Texture2D icon, Rectangle rect) : base(texture, rect)
        {
            _icon = icon;
        }

        public IconButton(Texture2D texture, IconData iconData, Rectangle rect) : base(texture, rect)
        {
            _icon = iconData.Spritesheet;
            _iconRect = iconData.SourceRect;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect, Color color)
        {
            spriteBatch.Draw(_icon, contentRect, _iconRect, color);
        }
    }
}