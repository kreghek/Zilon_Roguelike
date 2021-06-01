using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal abstract class ButtonBase
    {
        private const int CONTENT_MARGIN = 5;
        private readonly Rectangle _rect;
        private UiButtonState _buttonState;

        protected ButtonBase(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
            _rect = rect;
            _buttonState = UiButtonState.OutOfButton;
        }

        public Texture2D Texture { get; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var color = SelectColorByState();

            spriteBatch.Draw(Texture, _rect, color);

            var contentRect = new Rectangle(
                CONTENT_MARGIN + _rect.Left,
                CONTENT_MARGIN + _rect.Top,
                _rect.Width - (CONTENT_MARGIN * 2),
                _rect.Height - (CONTENT_MARGIN * 2));

            DrawContent(spriteBatch, contentRect, color);
        }

        public void Update()
        {
            var mouseState = Mouse.GetState();
            if (CheckMouseOver())
            {
                if (_buttonState == UiButtonState.Hover && mouseState.LeftButton == ButtonState.Pressed)
                {
                    _buttonState = UiButtonState.Pressed;
                }
                else if (mouseState.LeftButton == ButtonState.Released && _buttonState == UiButtonState.Pressed)
                {
                    _buttonState = UiButtonState.Hover;
                    OnClick?.Invoke(this, EventArgs.Empty);
                }
                else if (mouseState.LeftButton == ButtonState.Released)
                {
                    _buttonState = UiButtonState.Hover;
                }
            }
            else
            {
                _buttonState = UiButtonState.OutOfButton;
            }
        }

        protected abstract void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect, Color color);

        private bool CheckMouseOver()
        {
            var mouseState = Mouse.GetState();
            var mousePosition = mouseState.Position;

            var mouseRect = new Rectangle(mousePosition.X, mousePosition.Y, 1, 1);

            return _rect.Intersects(mouseRect);
        }

        private Color SelectColorByState()
        {
            var color = Color.White;
            return _buttonState switch
            {
                UiButtonState.OutOfButton => color, // Do not modify start color.
                UiButtonState.Hover => Color.Lerp(color, Color.Wheat, 0.25f),
                UiButtonState.Pressed => Color.Lerp(color, Color.Wheat, 0.75f),
                _ => Color.Red
            };
        }

        public event EventHandler? OnClick;
    }
}