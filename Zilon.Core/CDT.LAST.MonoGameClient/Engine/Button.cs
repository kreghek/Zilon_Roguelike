using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    class Button
    {
        private UiButtonState _buttonState;
        private readonly int _buttonX;
        private readonly int _buttonY;

        public int ButtonX
        {
            get
            {
                return _buttonX;
            }
        }

        public int ButtonY
        {
            get
            {
                return _buttonY;
            }
        }

        public string Name { get; }
        public Texture2D Texture { get; }

        public Action? Click { get; set; }

        public Button(string name, Texture2D texture, int buttonX, int buttonY)
        {
            Name = name;
            Texture = texture;
            _buttonX = buttonX;
            _buttonY = buttonY;
            _buttonState = UiButtonState.OutOfButton;
        }

        private bool CheckMouseOver()
        {
            var mouseState = Mouse.GetState();

            var mousePosition = mouseState.Position;
            if (mousePosition.X > _buttonX && mousePosition.X < _buttonX + Texture.Width &&
                mousePosition.Y > _buttonY && mousePosition.Y < _buttonY + Texture.Height)
            {
                return true;
            }
            return false;
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
                    Click?.Invoke();
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

            spriteBatch.Draw(Texture, new Rectangle(ButtonX, ButtonY, Texture.Width, Texture.Height), color);
        }
    }
}