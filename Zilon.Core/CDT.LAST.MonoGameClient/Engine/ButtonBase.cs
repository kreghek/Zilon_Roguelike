using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal abstract class ButtonBase
    {
        private readonly Rectangle _rect;
        private UiButtonState _buttonState;

        public ButtonBase(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
            _rect = rect;
            _buttonState = UiButtonState.OutOfButton;
        }

        public Texture2D Texture { get; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var color = Color.White;
            if (_buttonState == UiButtonState.OutOfButton)
            {
            }
            else if (_buttonState == UiButtonState.Hover)
            {
                color = Color.Lerp(color, Color.Wheat, 0.25f);
            }
            else if (_buttonState == UiButtonState.Pressed)
            {
                color = Color.Lerp(color, Color.Wheat, 0.75f);
            }
            else
            {
                color = Color.Red;
            }

            spriteBatch.Draw(Texture, _rect, color);
            var contentRect = new Rectangle(5 + _rect.Left, 5 + _rect.Top, _rect.Width - 5 * 2, _rect.Height - 5 * 2);
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

        public event EventHandler? OnClick;
    }
}