using System.Collections.Generic;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects
{
    internal sealed class HitEffect : IVisualEffect
    {
        private const double FPS = 1 / 12.0;
        private const double EFFECT_DISPLAY_DURATION_SECONDS = FPS * FRAME_COUNT;
        private const int FRAME_COUNT = 6;
        private const int FRAME_COLUMN_COUNT = 3;
        private const int FRAME_WIDTH = 128;
        private const int FRAME_HEIGHT = 64;

        private readonly Sprite _hitSprite;

        private readonly Texture2D _hitTexture;
        private readonly Texture2D _hitBackingTexture;
        private readonly Sprite _hitBackingSprite;

        private readonly GameObjectBase[] _boundGameObjects;

        private double _counter;

        public HitEffect(
            GameObjectBase attacker,
            GameObjectBase target,
            IGameObjectVisualizationContentStorage contentStorage,
            Vector2 targetObjectPosition,
            Vector2 direction)
        {
            _hitTexture = contentStorage.GetHitEffectTexture(HitEffectType.ShortBlade, HitEffectDirection.Left);
            _hitSprite = new Sprite(_hitTexture)
            {
                Position = targetObjectPosition,
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(255, 255, 255, 0.0f),
                SourceRectangle = new Rectangle(0, 0, FRAME_WIDTH, FRAME_HEIGHT)
            };

            _hitBackingTexture = contentStorage.GetHitEffectTexture(HitEffectType.ShortBlade | HitEffectType.Backing, HitEffectDirection.Left);
            _hitBackingSprite = new Sprite(_hitBackingTexture)
            {
                Position = targetObjectPosition,
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(255, 255, 255, 0.0f),
                SourceRectangle = new Rectangle(0, 0, FRAME_WIDTH, FRAME_HEIGHT)
            };

            if (direction.X > 0)
            {
                _hitSprite.FlipX = true;
                _hitBackingSprite.FlipX = true;
            }

            _counter = EFFECT_DISPLAY_DURATION_SECONDS;

            _boundGameObjects = new[] { attacker };
        }

        public bool IsComplete => _counter <= 0;

        public IEnumerable<GameObjectBase> BoundGameObjects => _boundGameObjects;

        public void Draw(SpriteBatch spriteBatch, bool backing)
        {
            if (!IsComplete)
            {
                if (backing)
                {
                    _hitBackingSprite.Draw(spriteBatch);
                }
                else
                {
                    _hitSprite.Draw(spriteBatch);
                }
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
                    frameColumn * FRAME_WIDTH,
                    frameRow * FRAME_HEIGHT,
                    FRAME_WIDTH,
                    FRAME_HEIGHT);
            }
        }
    }
}