using System;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Use for actions without any specific.
    /// Or when it is too boring to create new engine :)
    /// </summary>
    public sealed class ActorCommonActionEngine : IActorStateEngine
    {
        private const double ANIMATION_DURATION_SECONDS = 0.5;
        private readonly ICommandBlocker _animationBlocker;
        private readonly SoundEffectInstance? _consumeSoundEffect;
        private readonly SpriteContainer _rootContainer;
        private readonly Vector2 _startPosition;
        private readonly Vector2 _targetPosition;

        private double _animationCounterSeconds = ANIMATION_DURATION_SECONDS;

        private bool _effectPlayed;

        public ActorCommonActionEngine(SpriteContainer rootContainer, IAnimationBlockerService animationBlockerService,
            SoundEffectInstance? consumeSoundEffect)
        {
            _rootContainer = rootContainer;
            _consumeSoundEffect = consumeSoundEffect;

            _startPosition = rootContainer.Position;
            _targetPosition = _startPosition + (Vector2.UnitY * -10);

            _animationBlocker = new AnimationCommonBlocker();

            animationBlockerService.AddBlocker(_animationBlocker);
        }

        public bool IsComplete { get; private set; }

        public void Update(GameTime gameTime)
        {
            _animationCounterSeconds -= gameTime.ElapsedGameTime.TotalSeconds * 3;

            var progress = _animationCounterSeconds / ANIMATION_DURATION_SECONDS;

            var t = 1 - progress;

            if (_animationCounterSeconds > 0)
            {
                if (!_effectPlayed)
                {
                    _effectPlayed = true;

                    if (_consumeSoundEffect != null)
                    {
                        _consumeSoundEffect.Play();
                    }
                }

                var t2 = Math.Sin(t * Math.PI);
                _rootContainer.Position = Vector2.Lerp(_startPosition, _targetPosition, (float)t2);
            }
            else
            {
                _rootContainer.Position = _startPosition;
                _animationBlocker.Release();
                IsComplete = true;
            }
        }
    }
}