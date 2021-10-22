using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.GameComponents
{
    internal class FpsCounter : DrawableGameComponent
    {
        private const int MAXIMUM_SAMPLES = 100;

        private readonly SpriteFont _font;
        private readonly Queue<double> _sampleBuffer;
        private readonly SpriteBatch _spriteBatch;

        private double _averageFramesPerSecond;
        private double _currentFramesPerSecond;

        public FpsCounter(Game game, SpriteBatch spriteBatch, SpriteFont font) : base(game)
        {
            _spriteBatch = spriteBatch;
            _font = font;

            _sampleBuffer = new Queue<double>();
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateInner(gameTime);

            var fps = $"FPS: {_averageFramesPerSecond:00.00}";
            var size = _font.MeasureString(fps);

            const int SHADOW_OFFSET = 1;
            const int TEXT_MARGIN = 1;
            var textPosition = new Vector2(Game.GraphicsDevice.Viewport.Bounds.Right - size.X, 0);
            var textPositionWithMargin = textPosition + new Vector2(-TEXT_MARGIN, TEXT_MARGIN);
            var shadowOffset = new Vector2(SHADOW_OFFSET, SHADOW_OFFSET);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(
                _font,
                fps,
                textPositionWithMargin - shadowOffset,
                Color.Black);

            _spriteBatch.DrawString(
                _font,
                fps,
                textPositionWithMargin,
                Color.White);

            _spriteBatch.End();
        }

        private void UpdateInner(GameTime gameTime)
        {
            base.Update(gameTime);

            _currentFramesPerSecond = 1.0f / gameTime.ElapsedGameTime.TotalSeconds;

            _sampleBuffer.Enqueue(_currentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                _averageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                _averageFramesPerSecond = _currentFramesPerSecond;
            }
        }
    }
}