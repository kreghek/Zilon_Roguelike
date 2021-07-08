using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Zilon.Core.Client.Sector;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class ActorDamagedEngine : IActorStateEngine
    {
        private const float ANIMATION_DURATION_SECONDS = 2f;
        private readonly IActorGraphics _actorGraphics;
        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly SpriteContainer _graphicsRoot;
        private readonly ICommandBlocker _moveBlocker;
        private readonly SpriteContainer _rootSprite;

        private readonly Sprite _shadowSprite;
        private readonly SoundEffectInstance? _soundEffectInstance;
        private readonly Vector2 _startPosition;
        private readonly Vector2 _targetPosition;

        private double _animationCounterSeconds = ANIMATION_DURATION_SECONDS;

        public ActorDamagedEngine(IPerson person, IActorGraphics actorGraphics, SpriteContainer rootSprite, Vector2 hitPosition, IAnimationBlockerService animationBlockerService,
            SoundEffectInstance? soundEffectInstance)
        {
            _actorGraphics = actorGraphics;
            _rootSprite = rootSprite;
            _animationBlockerService = animationBlockerService;
            _soundEffectInstance = soundEffectInstance;
            _rootSprite.FlipX = (hitPosition - actorGraphics.RootSprite.Position).X < 0;

            //if (soundEffectInstance != null)
            //{
            //    _moveBlocker = new SoundAnimationBlocker(soundEffectInstance);
            //}
            //else
            //{
                _moveBlocker = new AnimationCommonBlocker();
            //}

            _animationBlockerService.AddBlocker(_moveBlocker);
        }

        public string? DebugName { get; set; }

        public bool IsComplete => _animationCounterSeconds <= 0;

        public void Update(GameTime gameTime)
        {
            _animationCounterSeconds -= gameTime.ElapsedGameTime.TotalSeconds * 3 * GameState.GameSpeed;
            var t = 1 - _animationCounterSeconds / ANIMATION_DURATION_SECONDS;

            if (t < 0.1f)
            {
            }
            else if (t >= 0.1f && t < 0.5f)
            {
                if (_soundEffectInstance != null && !_soundEffectInstance.IsDisposed &&
                _soundEffectInstance.State != SoundState.Playing)
                {
                    _soundEffectInstance.Play();
                }

                _actorGraphics.ShowHitlighted = true;
            }
            else
            {
                _actorGraphics.ShowHitlighted = false;
            }

            if (IsComplete)
            {
                _actorGraphics.ShowHitlighted = false;
                _moveBlocker.Release();
            }
        }
    }
}