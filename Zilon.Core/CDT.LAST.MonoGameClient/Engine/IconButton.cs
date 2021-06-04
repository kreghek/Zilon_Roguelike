using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal class IconButton : ButtonBase
    {
        private readonly Texture2D _icon;
        private readonly Rectangle? _iconRect;

        public IconButton(Texture2D texture, Texture2D icon, Rectangle rect) : this(texture, icon, rect, iconRect: null)
        {
        }

        public IconButton(Texture2D texture, Texture2D iconSpritesheet, Rectangle iconRect, Rectangle rect) : this(
            texture, iconSpritesheet, rect, iconRect: iconRect)
        {
        }

        private IconButton(Texture2D texture, Texture2D iconSpritesheet, Rectangle rect, Rectangle? iconRect = null) :
            base(texture, rect)
        {
            _icon = iconSpritesheet;
            _iconRect = iconRect;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect, Color color)
        {
            spriteBatch.Draw(_icon, contentRect, _iconRect, color);
        }
    }
}