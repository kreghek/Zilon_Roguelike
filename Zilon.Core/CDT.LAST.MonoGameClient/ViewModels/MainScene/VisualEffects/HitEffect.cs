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
        private const double EFFECT_DELAY_DURATION_SECONDS = 0.3;
        private const int FRAME_COUNT = 6;
        private const int FRAME_COLUMN_COUNT = 3;
        private const int FRAME_WIDTH = 128;
        private const int FRAME_HEIGHT = 64;

        private readonly GameObjectBase[] _boundGameObjects;
        private readonly Sprite _hitBackingSprite;

        private readonly Sprite _hitSprite;

        private double _animationCounter;
        private double _postAnimationCounter;

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

            var hitTexture = contentStorage.GetHitEffectTexture(hitType, hitDirection);
            _hitSprite = new Sprite(hitTexture)
            {
                Position = effectPosition,
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(255, 255, 255, 0.0f),
                SourceRectangle = new Rectangle(0, 0, FRAME_WIDTH, FRAME_HEIGHT)
            };

            var hitBackingTexture = contentStorage.GetHitEffectTexture(hitType | HitEffectType.Backing,
                HitEffectDirection.Left);
            _hitBackingSprite = new Sprite(hitBackingTexture)
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

            _animationCounter = EFFECT_DISPLAY_DURATION_SECONDS;
            _postAnimationCounter = EFFECT_DELAY_DURATION_SECONDS;

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
                    case "bite": return HitEffectType.Teeth;
                    default:
                        // Ignore other unknown tags. May be next tag gets more describtion about effect type.
                        break;
                }
            }

            Debug.Fail("Hit effect was not found to visualize combat action.");

            // Show default hit effect.
            return HitEffectType.ShortBlade;
        }

        public bool IsComplete => _animationCounter <= 0 && _postAnimationCounter <= 0;

        public IEnumerable<GameObjectBase> BoundGameObjects => _boundGameObjects;

        public void Draw(SpriteBatch spriteBatch, bool backing)
        {
            var isAnimationComplete = _animationCounter <= 0;

            if (!isAnimationComplete)
            {
                var spriteToDraw = backing ? _hitBackingSprite : _hitSprite;
                spriteToDraw.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            var isAnimationComplete = _animationCounter <= 0;
            if (!isAnimationComplete)
            {
                _animationCounter -= gameTime.ElapsedGameTime.TotalSeconds;

                var t = _animationCounter / EFFECT_DISPLAY_DURATION_SECONDS;
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
            else if (!IsComplete)
            {
                _postAnimationCounter -= gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}