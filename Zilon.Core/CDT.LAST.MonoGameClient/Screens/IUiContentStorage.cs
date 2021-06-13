using System;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Persons;

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
        Texture2D GetHintBackgroundTexture();
        PersonConditionTextures GetConditionIconTextures(IPersonCondition personCondition);
    }

    public record PersonConditionTextures
    {
        public PersonConditionTextures(Texture2D icon, Texture2D background)
        {
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            Background = background ?? throw new ArgumentNullException(nameof(background));
        }

        public Texture2D Icon { get; }
        public Texture2D Background { get; }
    }
}