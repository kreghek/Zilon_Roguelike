using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
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
}