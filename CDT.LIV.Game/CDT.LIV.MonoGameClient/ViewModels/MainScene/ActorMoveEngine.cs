
using System;

using CDT.LIV.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

using Zilon.Core.Client.Sector;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class ActorMoveEngine: IActorStateEngine
    {
        private double _moveCounter = 1f;

        private readonly Container _rootSprite;

        private Vector2 _targetPosition;
        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly ICommandBlocker _moveBlocker;
        private Vector2 _startPosition;

        public bool IsComplete => _moveCounter <= 0;

        public ActorMoveEngine(Container rootSprite, Vector2 targetPosition, IAnimationBlockerService animationBlockerService)
        {
            _rootSprite = rootSprite;
            _startPosition = rootSprite.Position;
            _targetPosition = targetPosition;
            _animationBlockerService = animationBlockerService;

            _moveBlocker = new AnimationCommonBlocker();

            _animationBlockerService.AddBlocker(_moveBlocker);
        }

        public void Update(GameTime gameTime)
        {
            _moveCounter -= gameTime.ElapsedGameTime.TotalSeconds * 3;
            var t = 1 - (float)_moveCounter;
            var stepAmplitude = 12f;
            var stepFrequncy = 2f;
            var unitVector = Vector2.UnitY * -1f;

            _rootSprite.Position = Vector2.Lerp(_startPosition, _targetPosition, t) + (float)Math.Abs(Math.Sin(t * Math.PI * stepFrequncy)) * unitVector * stepAmplitude;

            if (IsComplete)
            {
                _rootSprite.Position = _targetPosition;
                _moveBlocker.Release();
            }
        }
    }
}
