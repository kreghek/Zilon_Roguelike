using System;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Infinite animation of transition.
    /// It may be complete only througt DropBlocker in the blocker service.
    /// </summary>
    public sealed class ActorSectorTransitionMoveEngine : IActorStateEngine
    {
        private const double ANIMATION_DURATION_SECONDS = 0.5;
        private readonly ICommandBlocker _animationBlocker;
        private readonly SpriteContainer _rootContainer;
        private readonly SoundEffectInstance? _soundEffect;
        private readonly Vector2 _startPosition;
        private readonly Vector2 _targetPosition;

        private double _animationCounterSeconds = ANIMATION_DURATION_SECONDS;

        private bool _effectPlayed;

        public ActorSectorTransitionMoveEngine(SpriteContainer rootContainer,
            IAnimationBlockerService animationBlockerService,
            SoundEffectInstance? transitionSoundEffect)
        {
            _rootContainer = rootContainer;
            _soundEffect = transitionSoundEffect;

            _startPosition = rootContainer.Position;
            _targetPosition = _startPosition + (Vector2.UnitY * -10);

            if (transitionSoundEffect != null)
            {
                _animationBlocker = new SoundAnimationBlocker(transitionSoundEffect);
            }
            else
            {
                _animationBlocker = new AnimationCommonBlocker();
            }

            animationBlockerService.AddBlocker(_animationBlocker);
        }

        /// <inheritdoc />
        /// <remarks>
        /// The propperty has no setter because it is infinite.
        /// </remarks>
        public bool IsComplete { get; }

        public void Update(GameTime gameTime)
        {
            _animationCounterSeconds -= gameTime.ElapsedGameTime.TotalSeconds * 3;

            var progress = _animationCounterSeconds / ANIMATION_DURATION_SECONDS;

            var t = 1 - progress;

            if (IsAnimationContoniues())
            {
                if (!_effectPlayed)
                {
                    _effectPlayed = true;

                    if (_soundEffect != null && !_soundEffect.IsDisposed)
                    {
                        _soundEffect.IsLooped = true;
                        _soundEffect.Play();
                    }
                }

                var t2 = Math.Sin(t * Math.PI);
                _rootContainer.Position = Vector2.Lerp(_startPosition, _targetPosition, (float)t2);
            }
            else
            {
                _rootContainer.Position = _startPosition;
            }
        }

        private bool IsAnimationContoniues()
        {
            return _animationCounterSeconds > 0;
        }
    }
}