using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects
{
    public sealed class HitEffect : IVisualEffect
    {
        private const double EFFECT_DISPLAY_DURATION_SECONDS = 0.3f;
        private readonly Vector2 _direction;
        private readonly LivGame _game;
        private readonly Sprite _hitSprite;
        private double _counter;

        public HitEffect(LivGame game, Vector2 targetObjectPosition, Vector2 direction)
        {
            _game = game;
            _direction = direction;
            _hitSprite = new Sprite(_game.Content.Load<Texture2D>("Sprites/effects/hit"))
            {
                Position = targetObjectPosition,
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(255, 255, 255, 0.0f)
            };

            if (_direction.X < 0)
            {
                _hitSprite.FlipX = true;
            }

            _counter = EFFECT_DISPLAY_DURATION_SECONDS;
        }

        public bool IsComplete => _counter <= 0;

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsComplete)
            {
                _hitSprite.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            _counter -= gameTime.ElapsedGameTime.TotalSeconds;
            if (!IsComplete)
            {
                _hitSprite.ScaleScalar = 1 - (float)(_counter / EFFECT_DISPLAY_DURATION_SECONDS);
            }
        }
    }
}