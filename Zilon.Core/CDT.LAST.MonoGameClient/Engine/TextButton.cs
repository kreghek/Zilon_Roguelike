using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal class TextButton : ButtonBase
    {
        private readonly SpriteFont _font;

        public TextButton(string title, Texture2D texture, SpriteFont font, Rectangle rect) : base(texture, rect)
        {
            Title = title;

            _font = font;
        }

        public string Title { get; set; }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect, Color color)
        {
            var textSize = _font.MeasureString(Title);
            var widthDiff = contentRect.Width - textSize.X;
            var heightDiff = contentRect.Height - textSize.Y;
            var textPosition = new Vector2(
                (widthDiff / 2) + contentRect.Left,
                (heightDiff / 2) + contentRect.Top);

            spriteBatch.DrawString(_font, Title, textPosition, Color.White);
        }
    }
}