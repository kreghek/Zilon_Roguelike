using System;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class ActorPushEngine : IActorStateEngine
    {
        private const float ANIMATION_DURATION_SECONDS = 2f;
        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly SpriteContainer _graphicsRoot;
        private readonly ICommandBlocker _moveBlocker;
        private readonly SpriteContainer _rootSprite;

        private readonly Sprite _shadowSprite;
        private readonly SoundEffectInstance? _soundEffectInstance;
        private readonly Vector2 _startPosition;
        private readonly Vector2 _targetPosition;

        private double _animationCounterSeconds = ANIMATION_DURATION_SECONDS;

        public ActorPushEngine(SpriteContainer rootSprite, SpriteContainer graphicsRoot, Sprite shadowSprite,
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

        /// <inheritdoc />
        /// <remarks> The state engine has blocker. So we can't just replace it without blocker releasing. </remarks>
        public bool CanBeReplaced => false;

        public bool IsComplete => _animationCounterSeconds <= 0;

        public void Cancel()
        {
            _moveBlocker.Release();
        }

        public void Update(GameTime gameTime)
        {
            if (_soundEffectInstance != null && !_soundEffectInstance.IsDisposed &&
                _soundEffectInstance.State != SoundState.Playing)
            {
                _soundEffectInstance.Play();
            }

            _animationCounterSeconds -= gameTime.ElapsedGameTime.TotalSeconds * 3 * GameState.GameSpeed;
            var t = _animationCounterSeconds / ANIMATION_DURATION_SECONDS;
            var t2 = Math.Cos(t * Math.PI / 2);

            _rootSprite.Position = Vector2.Lerp(_startPosition, _targetPosition, (float)t2);

            if (IsComplete)
            {
                _rootSprite.Position = _targetPosition;
                _moveBlocker.Release();
            }
        }
    }
}