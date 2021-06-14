using System;

using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Helper structure to keep texture and metadata for head visualization.
    /// </summary>
    public record HeadPart
    {
        public HeadPart(HeadPartType type, Texture2D texture)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Type = type;
        }

        public Texture2D Texture { get; }

        public HeadPartType Type { get; }
    }
}