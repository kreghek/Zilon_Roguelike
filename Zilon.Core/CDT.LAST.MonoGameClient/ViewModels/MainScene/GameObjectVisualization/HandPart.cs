using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    /// <summary>
    /// Internal structure to store texture and metadata together.
    /// </summary>
    public record HandPart
    {
        public HandPart(HandPartType type, Texture2D texture)
        {
            Type = type;
            Texture = texture;
        }

        public Texture2D Texture { get; }

        public HandPartType Type { get; }
    }
}