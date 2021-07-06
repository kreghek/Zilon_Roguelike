using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class GameObjectVisualizationContentStorage : IGameObjectVisualizationContentStorage
    {
        private Texture2D? _consumingEffectTexture;
        private Dictionary<HitEffectKey, Texture2D>? _hitEffectDictionary;

        public Texture2D GetConsumingEffectTexture()
        {
            return _consumingEffectTexture ?? throw new InvalidOperationException("Effect spritesheet is not loaded.");
        }

        public Texture2D GetHitEffectTexture(HitEffectType effectType, HitEffectDirection effectDirection)
        {
            if (_hitEffectDictionary is null)
            {
                throw new InvalidOperationException("Hit effect textures is not loaded.");
            }

            var key = new HitEffectKey(effectType, effectDirection);

            if (_hitEffectDictionary.TryGetValue(key, out var texture))
            {
                return texture;
            }

            return _hitEffectDictionary[new HitEffectKey(HitEffectType.ShortBlade, HitEffectDirection.Left)];
        }

        public void LoadContent(ContentManager content)
        {
            _consumingEffectTexture = content.Load<Texture2D>("Sprites/VisualEffects/ConsumingEffects");
            _hitEffectDictionary = new Dictionary<HitEffectKey, Texture2D>
            {
                {
                    new HitEffectKey(HitEffectType.ShortBlade, HitEffectDirection.Left),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectType.ShortBlade, HitEffectDirection.LeftBottom),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectType.ShortBlade, HitEffectDirection.Right),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectType.ShortBlade, HitEffectDirection.RightBottom),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectType.ShortBlade, HitEffectDirection.TopLeft),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectType.ShortBlade, HitEffectDirection.TopRight),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                }
            };
        }

        private record HitEffectKey
        {
            public HitEffectKey(HitEffectType type, HitEffectDirection direction)
            {
                Type = type;
                Direction = direction;
            }

            public HitEffectDirection Direction { get; }

            public HitEffectType Type { get; }
        }
    }
}