﻿using System;
using System.Collections.Generic;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects
{
    internal sealed class ConsumingEffect : IVisualEffect
    {
        private const double EFFECT_DISPLAY_DURATION_SECONDS = 1f;
        private const int EFFECT_FLIGHT_DISTANCE = 40;
        private const int OFFSET_FREQUENCY = 3;
        private readonly GameObjectBase _bindGameObject;
        private readonly Sprite _effectSprite;
        private readonly Vector2 _startEffectPosition;
        private readonly Vector2 _targetEffectPosition;

        private double _counter;

        public ConsumingEffect(IGameObjectVisualizationContentStorage visualizationContentStorage,
            Vector2 targetObjectPosition, GameObjectBase bindGameObject, ConsumeEffectType effectType)
        {
            _startEffectPosition = targetObjectPosition;
            _bindGameObject = bindGameObject;
            _targetEffectPosition = targetObjectPosition - (Vector2.UnitY * EFFECT_FLIGHT_DISTANCE);

            var spriteSourceRect = GetSpriteSourceRect(effectType);

            _effectSprite = new Sprite(visualizationContentStorage.GetConsumingEffectTexture())
            {
                Position = targetObjectPosition,
                SourceRectangle = spriteSourceRect
            };

            _counter = EFFECT_DISPLAY_DURATION_SECONDS;
        }

        private static Rectangle GetSpriteSourceRect(ConsumeEffectType effectType)
        {
            const int SPRITE_SIZE = 16;

            return effectType switch
            {
                ConsumeEffectType.Eat => new Rectangle(0, 0, SPRITE_SIZE, SPRITE_SIZE),
                ConsumeEffectType.Drink => new Rectangle(SPRITE_SIZE, 0, SPRITE_SIZE, SPRITE_SIZE),
                ConsumeEffectType.Heal => new Rectangle(0, SPRITE_SIZE, SPRITE_SIZE, SPRITE_SIZE),
                _ => new Rectangle(SPRITE_SIZE, SPRITE_SIZE, SPRITE_SIZE, SPRITE_SIZE)
            };
        }

        public bool IsComplete => _counter <= 0;

        public IEnumerable<GameObjectBase> BoundGameObjects => new[] { _bindGameObject };

        public void Update(GameTime gameTime)
        {
            _counter -= gameTime.ElapsedGameTime.TotalSeconds;
            if (!IsComplete)
            {
                var t = 1 - (_counter / EFFECT_DISPLAY_DURATION_SECONDS);
                var verticalPosition = Vector2.Lerp(_startEffectPosition, _targetEffectPosition, (float)t);
                var horizontalOffset = new Vector2((float)Math.Sin(t * OFFSET_FREQUENCY * 2 * Math.PI), 0) * 3;
                _effectSprite.Position = verticalPosition + horizontalOffset;
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool backing)
        {
            if (backing)
            {
                // Consuming effects has no backing.
                return;
            }

            if (!IsComplete)
            {
                _effectSprite.Draw(spriteBatch);
            }
        }
    }
}