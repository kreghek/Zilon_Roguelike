using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal abstract class ButtonBase
    {
        private const int CONTENT_MARGIN = 0;
        private UiButtonState _buttonState;

        protected ButtonBase(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
            Rect = rect;
            _buttonState = UiButtonState.OutOfButton;
        }

        public Texture2D Texture { get; }

        public Rectangle Rect { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var color = SelectColorByState();

            spriteBatch.Draw(Texture, Rect, color);

            var contentRect = new Rectangle(
                CONTENT_MARGIN + Rect.Left,
                CONTENT_MARGIN + Rect.Top,
                Rect.Width - (CONTENT_MARGIN * 2),
                Rect.Height - (CONTENT_MARGIN * 2));

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
                    PlayClickSoundIfExists();
                }
                else if (mouseState.LeftButton == ButtonState.Released)
                {
                    if (_buttonState == UiButtonState.OutOfButton)
                    {
                        PlayHoverSoundIfExists();
                    }

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

            return Rect.Intersects(mouseRect);
        }

        private static void PlayClickSoundIfExists()
        {
            if (UiThemeManager.SoundStorage == null)
            {
                // See the description in PlayHoverSoundIfExists method.
                return;
            }

            var soundEffect = UiThemeManager.SoundStorage.GetButtonClickEffect();
            soundEffect.Play();
        }

        private static void PlayHoverSoundIfExists()
        {
            if (UiThemeManager.SoundStorage == null)
            {
                // Sound content was not loaded.
                // This may occured by a few reasons:
                // - Content was not loaded yet. This is not critical to skip effect playing once. May be next time sound effect will be ready to play.
                // - There is test environment. So there is need no sounds to auto-tests.
                // - Developer forget to load content and assign UiThemeManager.SoundStorage. This is error bu no way to differ is from two other reasons above.
                // So just do nothing.
                return;
            }

            var soundEffect = UiThemeManager.SoundStorage.GetButtonHoverEffect();
            soundEffect.Play();
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