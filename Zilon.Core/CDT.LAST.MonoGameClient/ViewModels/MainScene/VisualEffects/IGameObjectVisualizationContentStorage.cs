using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal interface IGameObjectVisualizationContentStorage
    {
        Texture2D GetConsumingEffectTexture();

        Texture2D GetHitEffectTexture(HitEffectType effectType, HitEffectDirection effectDirection);

        void LoadContent(ContentManager content);
    }

    internal enum HitEffectType
    {
        Undefined,
        ShortBlade
    }

    internal enum HitEffectDirection
    {
        Undefined,
        Left,
        TopLeft,
        TopRight,
        Right,
        RightBottom,
        LeftBottom
    }
}