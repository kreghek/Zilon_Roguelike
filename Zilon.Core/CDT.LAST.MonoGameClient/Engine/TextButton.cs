using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal class TextButton : ButtonBase
    {
        private readonly SpriteFont _font;

        public TextButton(string title, Texture2D texture, SpriteFont font, Rectangle rect): base(texture, rect)
        {
            Title = title;
            _font = font;
        }

        public string Title { get; set; }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect, Color color)
        {
            spriteBatch.DrawString(_font, Title, new Vector2(contentRect.Left, contentRect.Top), Color.Gray);
        }
    }
}