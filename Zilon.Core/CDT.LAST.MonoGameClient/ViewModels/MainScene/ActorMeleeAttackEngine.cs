using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class ActorMeleeAttackEngine : IActorStateEngine
    {
        private const double ANIMATION_DURATION_SECONDS = 0.5;
        private readonly ICommandBlocker _animationBlocker;
        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly IVisualEffect? _hitVisualEffect;

        private readonly SoundEffectInstance? _meleeAttackSoundEffect;
        private readonly SpriteContainer _rootContainer;

        private readonly Vector2 _startPosition;
        private readonly Vector2 _targetPosition;

        private double _animationCounterSeconds = ANIMATION_DURATION_SECONDS;
        private bool _effectPlayed;

        public ActorMeleeAttackEngine(SpriteContainer rootContainer, Vector2 targetPosition,
            IAnimationBlockerService animationBlockerService, SoundEffectInstance? meleeAttackSoundEffect,
            IVisualEffect? hitVisualEffect)
        {
            _rootContainer = rootContainer;
            _animationBlockerService = animationBlockerService;
            _meleeAttackSoundEffect = meleeAttackSoundEffect;
            _hitVisualEffect = hitVisualEffect;
            _startPosition = rootContainer.Position;
            _targetPosition = Vector2.Lerp(_startPosition, targetPosition, 0.6f);

            _animationBlocker = new AnimationCommonBlocker();

            _animationBlockerService.AddBlocker(_animationBlocker);

            _rootContainer.FlipX = (_startPosition - _targetPosition).X < 0;
        }

        public string? DebugName { get; set; }

        /// <inheritdoc />
        /// <remarks> The state engine has blocker. So we can't just replace it without blocker releasing. </remarks>
        public bool CanBeReplaced => false;

        public bool IsComplete { get; private set; }

        public void Cancel()
        {
            _animationBlocker.Release();
        }

        public void Update(GameTime gameTime)
        {
            if (_animationCounterSeconds > 0)
            {
                _animationCounterSeconds -= gameTime.ElapsedGameTime.TotalSeconds * 3;
                var t = 1 - _animationCounterSeconds / ANIMATION_DURATION_SECONDS;

                if (!_effectPlayed)
                {
                    _effectPlayed = true;

                    if (_meleeAttackSoundEffect != null)
                    {
                        _meleeAttackSoundEffect.Play();
                    }
                }

                var t2 = Math.Sin(t * Math.PI);
                _rootContainer.Position = Vector2.Lerp(_startPosition, _targetPosition, (float)t2);
            }
            else
            {
                _rootContainer.Position = _startPosition;

                if (_hitVisualEffect is not null)
                {
                    if (_hitVisualEffect.IsComplete)
                    {
                        _animationBlocker.Release();
                        IsComplete = true;
                    }
                }
                else
                {
                    // Nothing to wait.
                    // Complete move engine.
                    _animationBlocker.Release();
                    IsComplete = true;
                }
            }
        }
    }
}