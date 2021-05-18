using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    class Button
    {
        int buttonX, buttonY;

        public int ButtonX
        {
            get
            {
                return buttonX;
            }
        }

        public int ButtonY
        {
            get
            {
                return buttonY;
            }
        }

        public string Name { get; }
        public Texture2D Texture { get; }

        public Action Click { get; set; }

        private MouseState _lastMousState;

        public Button(string name, Texture2D texture, int buttonX, int buttonY)
        {
            Name = name;
            Texture = texture;
            this.buttonX = buttonX;
            this.buttonY = buttonY;
        }

        private bool CheckMouseOver()
        {
            var mouseState = Mouse.GetState();

            var mousePosition = mouseState.Position;
            if (mousePosition.X > buttonX && mousePosition.X < buttonX + Texture.Width &&
                mousePosition.Y > buttonY && mousePosition.Y < buttonY + Texture.Height)
            {
                return true;
            }
            return false;
        }

        public void Update()
        {
            var mouseState = Mouse.GetState();
            if (CheckMouseOver() && _lastMousState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {
                _lastMousState = mouseState;
                Click?.Invoke();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(ButtonX, ButtonY, Texture.Width, Texture.Height), Color.White);
        }
    }
}
