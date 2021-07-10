using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class ActorDamagedEngine : IActorStateEngine
    {
        private const float ANIMATION_DURATION_SECONDS = 1f;
        private const int HIT_DISTANCE = 12;
        private const float LOW_DELAY_PERCENT = 0.3f;
        private const float HI_DELAY_PERCENT = 0.6f;
        private readonly IActorGraphics _actorGraphics;
        private readonly Vector2 _hitPosition;
        private readonly ICommandBlocker _moveBlocker;
        private readonly SpriteContainer _rootSprite;

        private readonly SoundEffectInstance? _soundEffectInstance;
        private readonly Vector2 _startPosition;

        private double _animationCounterSeconds = ANIMATION_DURATION_SECONDS;
        private bool _soundPlayed;

        public ActorDamagedEngine(IActorGraphics actorGraphics, SpriteContainer rootSprite, Vector2 attackerPosition,
            IAnimationBlockerService animationBlockerService, SoundEffectInstance? soundEffectInstance)
        {
            _actorGraphics = actorGraphics;
            _rootSprite = rootSprite;
            _soundEffectInstance = soundEffectInstance;
            var positionDifference = attackerPosition - actorGraphics.RootSprite.Position;
            var hitDirection = positionDifference;
            hitDirection.Normalize();
            _rootSprite.FlipX = hitDirection.X > 0;
            _startPosition = rootSprite.Position;
            var oppositeDirection = hitDirection * -1;
            _hitPosition = (oppositeDirection * HIT_DISTANCE) + _startPosition;

            _moveBlocker = new AnimationCommonBlocker();

            animationBlockerService.AddBlocker(_moveBlocker);
        }

        public string? DebugName { get; set; }

        public bool IsComplete => _animationCounterSeconds <= 0 && _soundEffectInstance != null &&
                                  !_soundEffectInstance.IsDisposed &&
                                  _soundEffectInstance.State != SoundState.Stopped;

        /// <inheritdoc />
        /// <remarks> The state engine has blocker. So we can't just replace it without blocker releasing. </remarks>
        public bool CanBeReplaced => false;

        public void Cancel()
        {
            _moveBlocker.Release();
        }

        public void Update(GameTime gameTime)
        {
            _animationCounterSeconds -= gameTime.ElapsedGameTime.TotalSeconds * 3 * GameState.GameSpeed;
            var t = _animationCounterSeconds / ANIMATION_DURATION_SECONDS;
            var t2 = 1 - t;

            switch (t2)
            {
                case < LOW_DELAY_PERCENT:
                    // Do nothing. This is delay.
                    break;

                case >= LOW_DELAY_PERCENT and < HI_DELAY_PERCENT:
                    if (_soundEffectInstance != null && !_soundEffectInstance.IsDisposed 
                        && _soundEffectInstance.State != SoundState.Playing
                        && !_soundPlayed)
                    {
                        _soundEffectInstance.Play();
                        _soundPlayed = true;
                    }

                    _actorGraphics.ShowHitlighted = true;
                    _rootSprite.Position = _hitPosition;
                    break;

                default:
                    // Restore state after damage receiving animation.
                    _actorGraphics.ShowHitlighted = false;
                    _rootSprite.Position = _startPosition;
                    break;
            }

            if (IsComplete)
            {
                _actorGraphics.ShowHitlighted = false;
                _rootSprite.Position = _startPosition;
                _moveBlocker.Release();
            }
        }
    }
}