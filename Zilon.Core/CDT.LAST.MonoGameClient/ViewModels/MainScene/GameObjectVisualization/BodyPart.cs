using System;

using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    /// <summary>
    /// Helper structure to keep texture and metadata for body visualization.
    /// </summary>
    public record BodyPart
    {
        public BodyPart(BodyPartType type, Texture2D texture)
        {
            Type = type;
            Texture = texture;
        }

        public Texture2D Texture { get; }

        public BodyPartType Type { get; }
    }

    public record AnimalPart
    {
        public AnimalPart(AnimalPartType type, Texture2D texture)
        {
            Type = type;
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        }

        public Texture2D Texture { get; }

        public AnimalPartType Type { get; }
    }
}