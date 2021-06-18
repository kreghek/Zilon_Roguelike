using System;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class GameObjectVisualizationContentStorage : IGameObjectVisualizationContentStorage
    {
        private Texture2D? _consumingEffectTexture;

        public Texture2D GetConsumingEffectTexture()
        {
            return _consumingEffectTexture ?? throw new InvalidOperationException("Effect spritesheet is not loaded.");
        }

        public void LoadContent(ContentManager content)
        {
            _consumingEffectTexture = content.Load<Texture2D>("Sprites/effects/ConsumingEffects");
        }
    }
}