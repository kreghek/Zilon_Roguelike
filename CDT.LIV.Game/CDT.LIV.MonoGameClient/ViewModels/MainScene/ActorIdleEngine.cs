
using System;

using CDT.LIV.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class ActorIdleEngine : IActorStateEngine
    {
        private Random _random = new Random();
        private const double IDLE_CYCLE_DURATION_SECONDS = 0.75;
        private readonly Container _rootSprite;
        private double _idleAnimationCounter;
        private Vector2 _startPosition;
        private Vector2 _sourceStartPosition;
        private Vector2 _targetVector;
        private bool _toCenter;

        public ActorIdleEngine(Container rootSprite)
        {
            _startPosition = rootSprite.Position;
            _sourceStartPosition = _startPosition;
            _targetVector = GetUnitRandomVector();
            _rootSprite = rootSprite;
        }

        private Vector2 GetUnitRandomVector()
        {
            var a = (float)_random.NextDouble();
            var x = Math.Cos(a * Math.PI);
            var y = Math.Sin(a * Math.PI) * 0.5;

            var amplitude = 3f;

            return new Vector2((float)x, (float)y) * amplitude + _sourceStartPosition;
        }

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
                    _targetVector = _sourceStartPosition;
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
                _rootSprite.Position = positionInt;
            }
        }
    }
}
