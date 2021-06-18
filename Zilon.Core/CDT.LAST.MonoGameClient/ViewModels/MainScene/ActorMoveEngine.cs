using System;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class ActorMoveEngine : IActorStateEngine
    {
        private const float ANIMATION_DURATION_SECONDS = 1f;
        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly SpriteContainer _graphicsRoot;
        private readonly ICommandBlocker _moveBlocker;
        private readonly SpriteContainer _rootSprite;

        private readonly Sprite _shadowSprite;
        private readonly SoundEffectInstance? _soundEffectInstance;
        private readonly Vector2 _startPosition;
        private readonly Vector2 _targetPosition;

        private double _animationCounterSeconds = ANIMATION_DURATION_SECONDS;

        public ActorMoveEngine(SpriteContainer rootSprite, SpriteContainer graphicsRoot, Sprite shadowSprite,
            Vector2 targetPosition, IAnimationBlockerService animationBlockerService,
            SoundEffectInstance? soundEffectInstance)
        {
            _rootSprite = rootSprite;
            _graphicsRoot = graphicsRoot;
            _shadowSprite = shadowSprite;
            _startPosition = rootSprite.Position;
            _targetPosition = targetPosition;
            _animationBlockerService = animationBlockerService;
            _soundEffectInstance = soundEffectInstance;
            _rootSprite.FlipX = (_startPosition - _targetPosition).X < 0;

            if (soundEffectInstance != null)
            {
                _moveBlocker = new SoundAnimationBlocker(soundEffectInstance);
            }
            else
            {
                _moveBlocker = new AnimationCommonBlocker();
            }

            _animationBlockerService.AddBlocker(_moveBlocker);
        }

        public string? DebugName { get; set; }

        public bool IsComplete => _animationCounterSeconds <= 0;

        public void Update(GameTime gameTime)
        {
            if (_soundEffectInstance != null && !_soundEffectInstance.IsDisposed &&
                _soundEffectInstance.State != SoundState.Playing)
            {
                _soundEffectInstance.Play();
            }

            _animationCounterSeconds -= gameTime.ElapsedGameTime.TotalSeconds * 3 * GameState.GameSpeed;
            var t = 1 - _animationCounterSeconds / ANIMATION_DURATION_SECONDS;
            var stepAmplitude = 4f;
            var stepFrequncy = 2f;
            var unitVector = Vector2.UnitY * -1f;
            var stepCurrentValue = (float)Math.Abs(Math.Sin(t * Math.PI * stepFrequncy));

            _rootSprite.Position = Vector2.Lerp(_startPosition, _targetPosition, (float)t);
            _graphicsRoot.Position = stepCurrentValue * unitVector * stepAmplitude;
            _shadowSprite.ScaleScalar = stepCurrentValue * 0.5f + 0.5f;

            if (IsComplete)
            {
                _rootSprite.Position = _targetPosition;
                _shadowSprite.ScaleScalar = 1;
                _moveBlocker.Release();
            }
        }
    }
}