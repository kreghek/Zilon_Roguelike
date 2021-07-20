using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.Screens
{
    public interface IUiContentStorage
    {
        Texture2D GetAttributeBackgroundTexture();

        Texture2D GetAttributeIconsTexture();
        SpriteFont GetAuxTextFont();
        Texture2D GetBottomPanelBackground();
        SpriteFont GetButtonFont();
        Texture2D GetButtonTexture();
        Texture2D GetCombatActIconTexture(string? sid, string[] tags);
        PersonConditionTextures GetConditionIconTextures(IPersonCondition personCondition);
        Texture2D GetContextualMenuBorderTexture();
        Texture2D GetHintBackgroundTexture();

        SpriteFont GetHintTitleFont();
        SpriteFont GetMenuItemFont();
        Texture2D GetMenuItemTexture();

        Texture2D[] GetModalBottomTextures();

        Texture2D GetModalShadowTexture();

        Texture2D[] GetModalTopTextures();

        Texture2D GetPersonMarkerTextureSheet();

        Texture2D[] GetPropIconLayers(string sid);
        Texture2D GetSelectedButtonMarkerTexture();
        Texture2D GetSmallVerticalButtonBackgroundTexture();
        Texture2D GetSmallVerticalButtonIconsTexture();
        void LoadContent(ContentManager contentManager);
    }
}