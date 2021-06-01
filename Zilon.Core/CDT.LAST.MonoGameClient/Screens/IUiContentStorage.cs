
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Screens
{
    public interface IUiContentStorage
    {
        Texture2D GetButtonTexture();

        SpriteFont GetButtonFont();

        SpriteFont GetHintTitleFont();

        Texture2D[] GetModalTopTextures();

        Texture2D[] GetModalBottomTextures();

        Texture2D GetModalShadowTexture();

        Texture2D[] GetPropIconLayers(string sid);

        void LoadContent(ContentManager contentManager);
    }
}