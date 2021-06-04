using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal class TextButton : ButtonBase
    {
        private readonly SpriteFont _font;
        private readonly Vector2 _textPosition;

        public TextButton(string title, Texture2D texture, SpriteFont font, Rectangle rect) : base(texture, rect)
        {
            Title = title;

            _font = font;

            var textSize = _font.MeasureString(Title);
            var widthDiff = rect.Width - textSize.X;
            var heightDiff = rect.Height - textSize.Y;
            _textPosition = new Vector2(
                (widthDiff / 2) + rect.Left,
                (heightDiff / 2) + rect.Top);
        }

        public string Title { get; set; }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect, Color color)
        {
            spriteBatch.DrawString(_font, Title, _textPosition, Color.Gray);
        }
    }
}