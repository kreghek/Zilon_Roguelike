using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class HitEffect
    {
        private const double EFFECT_DISPLAY_DURATION_SECONDS = 0.3f;
        private readonly Vector2 _direction;
        private readonly LivGame _game;
        private readonly Sprite _hitSprite;
        private double _counter;

        public HitEffect(LivGame game, Vector2 targetSpritePosition, Vector2 direction)
        {
            _game = game;
            _direction = direction;
            _hitSprite = new Sprite(_game.Content.Load<Texture2D>("Sprites/effects/hit"))
            {
                Position = targetSpritePosition,
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
                //_hitSprite.Color = new Color(Color.White, /*(float)(_counter / EFFECT_DISPLAY_DURATION_SECONDS) * 0.5f +*/ 0.25f);
                _hitSprite.ScaleScalar = 1 - (float)(_counter / EFFECT_DISPLAY_DURATION_SECONDS);
            }
        }
    }
}