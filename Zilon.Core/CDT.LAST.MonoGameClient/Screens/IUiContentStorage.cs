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
        PersonConditionTextures GetConditionIconTextures(IPersonCondition personCondition);
        Texture2D GetContextualMenuBorderTexture();
        Texture2D GetHintBackgroundTexture();

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

    public record PersonConditionTextures
    {
        public PersonConditionTextures(Texture2D icon, Texture2D background)
        {
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            Background = background ?? throw new ArgumentNullException(nameof(background));
        }

        public Texture2D Background { get; }

        public Texture2D Icon { get; }
    }
}