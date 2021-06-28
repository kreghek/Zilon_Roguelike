using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public interface IGameObjectVisualizationContentStorage
    {
        Texture2D GetConsumingEffectTexture();
        void LoadContent(ContentManager content);
    }
}