using System;
using System.Collections.Generic;
using System.Diagnostics;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Persons;

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

        private readonly GameObjectBase[] _boundGameObjects;
        private readonly Sprite _hitBackingSprite;
        private readonly Texture2D _hitBackingTexture;

        private readonly Sprite _hitSprite;

        private readonly Texture2D _hitTexture;

        private double _counter;

        public HitEffect(
            GameObjectBase attacker,
            GameObjectBase target,
            IGameObjectVisualizationContentStorage contentStorage,
            Vector2 effectPosition,
            Vector2 direction,
            ActDescription usedActDescription)
        {
            var hitType = GetHitType(usedActDescription);
            var hitDirection = GetHitDirection(direction);

            _hitTexture = contentStorage.GetHitEffectTexture(hitType, hitDirection);
            _hitSprite = new Sprite(_hitTexture)
            {
                Position = effectPosition,
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(255, 255, 255, 0.0f),
                SourceRectangle = new Rectangle(0, 0, FRAME_WIDTH, FRAME_HEIGHT)
            };

            _hitBackingTexture = contentStorage.GetHitEffectTexture(hitType | HitEffectType.Backing,
                HitEffectDirection.Left);
            _hitBackingSprite = new Sprite(_hitBackingTexture)
            {
                Position = effectPosition,
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

            _boundGameObjects = new[] { attacker, target };
        }

        private static HitEffectDirection GetHitDirection(Vector2 direction)
        {
            var startDirection = HitEffectDirection.Left;

            if (direction.Y < 0)
            {
                return HitEffectDirection.Top | startDirection;
            }

            if (direction.Y > 0)
            {
                return HitEffectDirection.Bottom | startDirection;
            }

            return startDirection;
        }

        private static HitEffectType GetHitType(ActDescription usedActDescription)
        {
            foreach (var tag in usedActDescription.Tags)
            {
                switch (tag)
                {
                    case "slash": return HitEffectType.ShortBlade;
                }
            }

            Debug.Fail("Hit effect was not found to visualize combat action.");

            // Show default hit effect.
            return HitEffectType.ShortBlade;
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