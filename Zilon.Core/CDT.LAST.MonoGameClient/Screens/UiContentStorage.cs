using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal sealed class UiContentStorage : IUiContentStorage
    {
        private readonly Dictionary<string, Texture2D[]> _propIcons;
        private Texture2D? _attributeIconsTexture;
        private Texture2D? _attributesBackgroundTexture;
        private SpriteFont? _buttonFont;
        private Texture2D? _buttonTexture;
        private SpriteFont? _hintTitleFont;
        private Texture2D[]? _modalBottomTextures;
        private Texture2D? _modalShadowTexture;
        private Texture2D[]? _modalTopTextures;
        private Texture2D? _smallVerticalButtonBackgroundTexture;
        private Texture2D? _smallVerticalButtonIconsTexture;
        private Texture2D? _contextualMenuBorderTexture;
        private SpriteFont? _contextualMenuItemFont;
        private Texture2D? _contextualMenuItemBackgroundTexture;

        public UiContentStorage()
        {
            _propIcons = new Dictionary<string, Texture2D[]>();
        }

        public Texture2D GetSmallVerticalButtonBackgroundTexture()
        {
            return _smallVerticalButtonBackgroundTexture ?? throw new InvalidOperationException();
        }

        public SpriteFont GetButtonFont()
        {
            return _buttonFont ?? throw new InvalidOperationException();
        }

        public Texture2D GetButtonTexture()
        {
            return _buttonTexture ?? throw new InvalidOperationException();
        }

        public Texture2D[] GetModalBottomTextures()
        {
            return _modalBottomTextures ?? throw new InvalidOperationException();
        }

        public Texture2D[] GetModalTopTextures()
        {
            return _modalTopTextures ?? throw new InvalidOperationException();
        }

        public Texture2D[] GetPropIconLayers(string sid)
        {
            if (_propIcons.TryGetValue(sid, out var propTextureList))
            {
                return propTextureList;
            }

            Debug.Fail("Each prop in the game must have your own icon.");
            return _propIcons["EmptyPropIcon"];
        }

        public Texture2D GetModalShadowTexture()
        {
            return _modalShadowTexture ?? throw new InvalidOperationException();
        }

        public void LoadContent(ContentManager contentManager)
        {
            _buttonFont = contentManager.Load<SpriteFont>("Fonts/Main");
            _hintTitleFont = contentManager.Load<SpriteFont>("Fonts/HintTitle");
            _buttonTexture = contentManager.Load<Texture2D>("Sprites/ui/button");
            _modalShadowTexture = contentManager.Load<Texture2D>("Sprites/ui/ModalDialogShadow");
            _modalTopTextures = new[] { contentManager.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundTop1") };
            _modalBottomTextures = new[] { contentManager.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundBottom1") };
            _attributeIconsTexture = contentManager.Load<Texture2D>("Sprites/ui/AttributeIcons");
            _attributesBackgroundTexture = contentManager.Load<Texture2D>("Sprites/ui/AttributesBackground");
            _smallVerticalButtonIconsTexture = contentManager.Load<Texture2D>("Sprites/ui/SmallVerticalButtonIcons");
            _smallVerticalButtonBackgroundTexture =
                contentManager.Load<Texture2D>("Sprites/ui/SmallVerticalButtonBackground");

            _contextualMenuItemFont = contentManager.Load<SpriteFont>("Fonts/ContextualMenu");
            _contextualMenuBorderTexture = contentManager.Load<Texture2D>("Sprites/ui/ContextualMenuBorder");
            _contextualMenuItemBackgroundTexture = contentManager.Load<Texture2D>("Sprites/ui/ContextualMenuItemBackground");

            // Place textures in order to display. Latest will display on the top.
            _propIcons.Add("short-sword",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/ShortSword")
                });
            _propIcons.Add("great-sword",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/GreatSword")
                });
            _propIcons.Add("combat-staff",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/CombatStaff")
                });
            _propIcons.Add("tribal-spear",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/Spear")
                });
            _propIcons.Add("wooden-shield",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/WoodenShield")
                });
            _propIcons.Add("work-clothes",
                new[] { contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/BodyParts/WorkClothes/Icon") });
            _propIcons.Add("traveler-camisole",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/BodyParts/TravelerCamisole/Icon")
                });
            _propIcons.Add("knitted-hat",
                new[] { contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/HeadParts/KnittedHatIcon") });
            _propIcons.Add("steel-helmet",
                new[] { contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/HeadParts/SteelHelmetIcon") });

            _propIcons.Add("med-kit",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/MedKit") });
            _propIcons.Add("water-bottle",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/Waterskin") });
            _propIcons.Add("packed-food",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/PackedFood") });

            _propIcons.Add("EmptyPropIcon", new[] { contentManager.Load<Texture2D>("Sprites/ui/EmptyPropIcon") });
        }

        public SpriteFont GetHintTitleFont()
        {
            return _hintTitleFont ?? throw new InvalidOperationException();
        }

        public Texture2D GetAttributeIconsTexture()
        {
            return _attributeIconsTexture ?? throw new InvalidOperationException();
        }

        public Texture2D GetSmallVerticalButtonIconsTexture()
        {
            return _smallVerticalButtonIconsTexture ?? throw new InvalidOperationException();
        }

        public Texture2D GetAttributeBackgroundTexture()
        {
            return _attributesBackgroundTexture ?? throw new InvalidOperationException();
        }

        public Texture2D GetContextualMenuBorderTexture()
        {
            return _contextualMenuBorderTexture ?? throw new InvalidOperationException();
        }

        public SpriteFont GetMenuItemFont()
        {
            return _contextualMenuItemFont ?? throw new InvalidOperationException();
        }

        public Texture2D GetMenuItemTexture()
        {
            return _contextualMenuItemBackgroundTexture ?? throw new InvalidOperationException();
        }
    }
}