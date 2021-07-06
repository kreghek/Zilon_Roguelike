using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects
{
    internal sealed class HitEffect : IVisualEffect
    {
        private const double EFFECT_DISPLAY_DURATION_SECONDS = 0.3f;
        private const int FRAME_COUNT = 6;
        private const int FRAME_COLUMN_COUNT = 3;
        private const int FRAME_ROW_COUNT = 2;
        private const int FRAME_SIZE = 64;

        private readonly Sprite _hitSprite;

        private readonly Texture2D _hitTexture;
        private readonly Rectangle _sourceRect;
        private double _counter;

        public HitEffect(IGameObjectVisualizationContentStorage contentStorage, Vector2 targetObjectPosition,
            Vector2 direction)
        {
            _hitTexture = contentStorage.GetHitEffectTexture(HitEffectType.ShortBlade, HitEffectDirection.Left);
            _sourceRect = new Rectangle(0, 0, 64, 64);
            _hitSprite = new Sprite(_hitTexture)
            {
                Position = targetObjectPosition,
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(255, 255, 255, 0.0f),
                SourceRectangle = _sourceRect
            };

            if (direction.X > 0)
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
                var t = _counter / EFFECT_DISPLAY_DURATION_SECONDS;
                var t2 = 1 - t;

                var frameIndex = (int)(FRAME_COUNT * t2);
                var frameColumn = frameIndex % FRAME_COLUMN_COUNT;
                var frameRow = frameIndex / FRAME_COLUMN_COUNT;

                _hitSprite.SourceRectangle = new Rectangle(
                    frameColumn * FRAME_SIZE,
                    frameRow * FRAME_SIZE,
                    FRAME_SIZE,
                    FRAME_SIZE);
            }
        }
    }
}