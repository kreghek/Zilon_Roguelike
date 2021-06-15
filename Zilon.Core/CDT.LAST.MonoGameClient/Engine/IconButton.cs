using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal sealed class IconButton : ButtonBase
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

    internal sealed class CombatActButton : ButtonBase
    {
        private readonly Texture2D _icon;
        private readonly Texture2D _selectedMarker;
        private readonly CombatActButtonGroup _buttonGroup;

        public CombatActButton(Texture2D texture, Texture2D icon, Texture2D selectedMarker, CombatActButtonGroup buttonGroup, Rectangle rect) : base(texture, rect)
        {
            _icon = icon;
            _selectedMarker = selectedMarker;
            _buttonGroup = buttonGroup;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect, Color color)
        {
            if (_buttonGroup.Selected == this)
            {
                spriteBatch.Draw(_selectedMarker, contentRect, color);
            }

            spriteBatch.Draw(_icon, contentRect, color);
        }
    }

    internal sealed class CombatActButtonGroup
    { 
        public CombatActButton? Selected { get; set; }
    }
}