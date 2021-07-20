using System;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class ActorIdleEngine : IActorStateEngine
    {
        private const double IDLE_CYCLE_DURATION_SECONDS = 0.75;
        private readonly SpriteContainer _graphicsRoot;
        private readonly Random _random;
        private double _idleAnimationCounter;
        private Vector2 _startPosition;
        private Vector2 _targetVector;
        private bool _toCenter;

        public ActorIdleEngine(SpriteContainer graphicsRoot)
        {
            _graphicsRoot = graphicsRoot;

            _random = new Random();

            _startPosition = graphicsRoot.Position;
            _targetVector = GetUnitRandomVector();
        }

        private Vector2 GetUnitRandomVector()
        {
            var a = (float)_random.NextDouble();
            var x = Math.Cos(a * Math.PI);
            var y = Math.Sin(a * Math.PI) * 0.5;

            var amplitude = 3f;

            return new Vector2((float)x, (float)y) * amplitude;
        }

        /// <inheritdoc />
        /// <remarks> The state engine has no blockers. So we can't remove it with no aftermaths. </remarks>
        public bool CanBeReplaced => true;

        /// <summary>
        /// This engine is infinite.
        /// </summary>
        public bool IsComplete => false;

        public void Update(GameTime gameTime)
        {
            _idleAnimationCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (_idleAnimationCounter >= IDLE_CYCLE_DURATION_SECONDS)
            {
                _toCenter = !_toCenter;
                _startPosition = _targetVector;
                if (_toCenter)
                {
                    _targetVector = Vector2.Zero;
                }
                else
                {
                    _targetVector = GetUnitRandomVector();
                }

                _targetVector = GetUnitRandomVector();
                _idleAnimationCounter = 0;
            }
            else
            {
                var t = _idleAnimationCounter / IDLE_CYCLE_DURATION_SECONDS;

                var positionFloat = Vector2.Lerp(_startPosition, _targetVector, (float)t);
                // Round vector to exclude sprite smooth then sprite is not in integer position
                var positionInt = new Vector2((int)positionFloat.X, (int)positionFloat.Y);
                _graphicsRoot.Position = positionInt;
            }
        }

        public void Cancel()
        {
            // There is no blockers. So do nothing.
        }
    }
}