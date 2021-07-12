using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal interface IGameObjectVisualizationContentStorage
    {
        Texture2D GetConsumingEffectTexture();

        Texture2D GetHitEffectTexture(HitEffectTypes effectType, HitEffectDirections effectDirection);

        void LoadContent(ContentManager content);
    }
}