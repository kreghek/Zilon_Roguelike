using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Screens
{
    public interface IUiContentStorage
    {
        Texture2D GetAttributeBackgroundTexture();

        Texture2D GetAttributeIconsTexture();
        SpriteFont GetButtonFont();
        Texture2D GetButtonTexture();
        Texture2D GetContextualMenuBorderTexture();

        SpriteFont GetHintTitleFont();
        SpriteFont GetMenuItemFont();
        Texture2D GetMenuItemTexture();

        Texture2D[] GetModalBottomTextures();

        Texture2D GetModalShadowTexture();

        Texture2D[] GetModalTopTextures();

        Texture2D[] GetPropIconLayers(string sid);
        Texture2D GetSmallVerticalButtonBackgroundTexture();
        Texture2D GetSmallVerticalButtonIconsTexture();
        void LoadContent(ContentManager contentManager);
    }
}