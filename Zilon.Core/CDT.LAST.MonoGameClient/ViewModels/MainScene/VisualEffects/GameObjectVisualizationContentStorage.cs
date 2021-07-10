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

        public Texture2D GetHitEffectTexture(HitEffectTypes effectType, HitEffectDirections effectDirection)
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

            return _hitEffectDictionary[new HitEffectKey(HitEffectTypes.ShortBlade, HitEffectDirections.Left)];
        }

        public void LoadContent(ContentManager content)
        {
            _consumingEffectTexture = content.Load<Texture2D>("Sprites/VisualEffects/ConsumingEffects");

            _hitEffectDictionary = new Dictionary<HitEffectKey, Texture2D>
            {
                {
                    new HitEffectKey(HitEffectTypes.ShortBlade, HitEffectDirections.Left),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectTypes.ShortBlade, HitEffectDirections.Left | HitEffectDirections.Top),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectTypes.ShortBlade, HitEffectDirections.Left | HitEffectDirections.Bottom),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectTypes.ShortBlade | HitEffectTypes.Backing, HitEffectDirections.Left),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffectBaking")
                },

                {
                    new HitEffectKey(HitEffectTypes.ShortBlade | HitEffectTypes.Backing,
                        HitEffectDirections.Left | HitEffectDirections.Top),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffectBaking")
                },

                {
                    new HitEffectKey(HitEffectTypes.ShortBlade | HitEffectTypes.Backing,
                        HitEffectDirections.Left | HitEffectDirections.Bottom),
                    content.Load<Texture2D>("Sprites/VisualEffects/BladeShortHorizontalHitEffectBaking")
                },

                {
                    new HitEffectKey(HitEffectTypes.Teeth, HitEffectDirections.Left),
                    content.Load<Texture2D>("Sprites/VisualEffects/TeethHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectTypes.Teeth, HitEffectDirections.Left | HitEffectDirections.Top),
                    content.Load<Texture2D>("Sprites/VisualEffects/TeethHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectTypes.Teeth, HitEffectDirections.Left | HitEffectDirections.Bottom),
                    content.Load<Texture2D>("Sprites/VisualEffects/TeethHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectTypes.Teeth | HitEffectTypes.Backing, HitEffectDirections.Left),
                    content.Load<Texture2D>("Sprites/VisualEffects/TeethHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectTypes.Teeth | HitEffectTypes.Backing,
                        HitEffectDirections.Left | HitEffectDirections.Top),
                    content.Load<Texture2D>("Sprites/VisualEffects/TeethHorizontalHitEffect")
                },

                {
                    new HitEffectKey(HitEffectTypes.Teeth | HitEffectTypes.Backing,
                        HitEffectDirections.Left | HitEffectDirections.Bottom),
                    content.Load<Texture2D>("Sprites/VisualEffects/TeethHorizontalHitEffect")
                }
            };
        }

        private record HitEffectKey
        {
            public HitEffectKey(HitEffectTypes type, HitEffectDirections direction)
            {
                Type = type;
                Direction = direction;
            }

            public HitEffectDirections Direction { get; }

            public HitEffectTypes Type { get; }
        }
    }
}