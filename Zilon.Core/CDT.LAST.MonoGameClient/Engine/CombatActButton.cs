using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal sealed class CombatActButton : ButtonBase
    {
        private readonly CombatActButtonGroup _buttonGroup;
        private readonly Texture2D _icon;
        private readonly Texture2D _selectedMarker;

        public CombatActButton(Texture2D texture, Texture2D icon, Texture2D selectedMarkerTexture,
            CombatActButtonGroup buttonGroup, Rectangle rect) : base(texture, rect)
        {
            _icon = icon;
            _selectedMarker = selectedMarkerTexture;
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
}