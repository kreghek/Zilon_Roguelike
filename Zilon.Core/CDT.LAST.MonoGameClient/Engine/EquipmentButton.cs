using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal class EquipmentButton : ButtonBase
    {
        private readonly Texture2D[] _iconLayers;
        private readonly Rectangle _sourceRect;

        public EquipmentButton(Texture2D texture, Texture2D[] iconLayers, Rectangle rect, Rectangle sourceRect) : base(
            texture, rect)
        {
            _iconLayers = iconLayers;
            _sourceRect = sourceRect;
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect, Color color)
        {
            foreach (var iconLayer in _iconLayers)
            {
                spriteBatch.Draw(iconLayer, contentRect, _sourceRect, color);
            }
        }
    }
}