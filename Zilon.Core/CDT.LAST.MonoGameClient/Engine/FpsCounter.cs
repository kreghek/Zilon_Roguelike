using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal class FpsCounter: DrawableGameComponent
    {
        public FpsCounter(Game game, SpriteBatch spriteBatch, SpriteFont font) : base(game)
        {
            _spriteBatch = spriteBatch;
            _font = font;
        }

        public long TotalFrames { get; private set; }
        public double TotalSeconds { get; private set; }
        public double AverageFramesPerSecond { get; private set; }
        public double CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private readonly Queue<double> _sampleBuffer = new Queue<double>();

        public void UpdateInner(GameTime gameTime)
        {
            base.Update(gameTime);

            CurrentFramesPerSecond = 1.0f / gameTime.ElapsedGameTime.TotalSeconds;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateInner(gameTime);

            var fps = $"FPS: {AverageFramesPerSecond:00.00}";
            var size = _font.MeasureString(fps);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(
                _font,
                fps,
                new Vector2(Game.GraphicsDevice.Viewport.Bounds.Right - size.X - 1, 1),
                Color.Black);
            _spriteBatch.End();
        }
    }
}
