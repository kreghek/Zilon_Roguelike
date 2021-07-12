using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class CorpseViewModel
    {
        private const double CORPSE_DURATION_SECONDS = 5;
        private readonly IActorGraphics _actorGraphics;
        private readonly SpriteContainer _rootSprite;
        private readonly Microsoft.Xna.Framework.Audio.SoundEffectInstance _soundEffectInstance;

        private double _counter;
        private bool _rotated;
        private bool _soundPlayed;

        public CorpseViewModel(Game game,
            IActorGraphics actorGraphics,
            Vector2 position,
            Microsoft.Xna.Framework.Audio.SoundEffectInstance soundEffectInstance)
        {
            _actorGraphics = actorGraphics ?? throw new ArgumentNullException(nameof(actorGraphics));
            _soundEffectInstance = soundEffectInstance;
            var shadowTexture = game.Content.Load<Texture2D>("Sprites/game-objects/simple-object-shadow");

            _rootSprite = new SpriteContainer
            {
                Position = position
            };

            var shadowSprite = new Sprite(shadowTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(Color.White, 0.5f)
            };

            _rootSprite.AddChild(shadowSprite);

            _actorGraphics.ShowOutlined = false;
            _actorGraphics.ShowHitlighted = false;
            _rootSprite.AddChild(_actorGraphics.RootSprite);
        }

        public bool IsComplete => _counter >= CORPSE_DURATION_SECONDS;

        public void Draw(SpriteBatch spriteBatch)
        {
            _rootSprite.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            if (!IsComplete)
            {
                _counter += gameTime.ElapsedGameTime.TotalSeconds;

                if (!_rotated)
                {
                    _actorGraphics.RootSprite.Rotation = (float)(Math.PI / 2);
                    _rotated = true;
                }

                if (!_soundPlayed && _soundEffectInstance != null &&
                    _soundEffectInstance.State != Microsoft.Xna.Framework.Audio.SoundState.Playing)
                {
                    _soundPlayed = true;
                    _soundEffectInstance.Play();
                }
            }
        }
    }
}