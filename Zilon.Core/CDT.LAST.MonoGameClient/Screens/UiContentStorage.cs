using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Screens
{
    sealed class UiContentStorage : IUiContentStorage
    {
        private Texture2D? _buttonTexture;
        private Texture2D[]? _modalTopTextures;
        private Texture2D[]? _modalBottomTextures;
        private Texture2D? _modalShadowTexture;
        private SpriteFont? _buttonFont;
        private SpriteFont? _hintTitleFont;
        private readonly Dictionary<string, Texture2D[]> _propIcons;

        public UiContentStorage()
        {
            _propIcons = new Dictionary<string, Texture2D[]>();
        }

        public SpriteFont GetButtonFont() => _buttonFont ?? throw new InvalidOperationException();

        public Texture2D GetButtonTexture() => _buttonTexture ?? throw new InvalidOperationException();

        public Texture2D[] GetModalBottomTextures() => _modalBottomTextures ?? throw new InvalidOperationException();

        public Texture2D[] GetModalTopTextures() => _modalTopTextures ?? throw new InvalidOperationException();

        public Texture2D[] GetPropIconLayers(string sid)
        {
            if (_propIcons.TryGetValue(sid, out var propTextureList))
            {
                return propTextureList;
            }
            else
            {
                Debug.Fail("Each prop in the game must have your own icon.");
                return _propIcons["EmptyPropIcon"];
            }
        }

        public Texture2D GetModalShadowTexture() => _modalShadowTexture ?? throw new InvalidOperationException();

        public void LoadContent(ContentManager contentManager)
        {
            _buttonFont = contentManager.Load<SpriteFont>("Fonts/Main");
            _hintTitleFont = contentManager.Load<SpriteFont>("Fonts/HintTitle");
            _buttonTexture = contentManager.Load<Texture2D>("Sprites/ui/button");
            _modalShadowTexture = contentManager.Load<Texture2D>("Sprites/ui/ModalDialogShadow");
            _modalTopTextures = new[] { contentManager.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundTop1") };
            _modalBottomTextures = new[] { contentManager.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundBottom1") };

            // Place textures in order to display. Latest will display on the top.
            _propIcons.Add("short-sword", new[] { contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/ShortSwordBase") });
            _propIcons.Add("wooden-shield", new[] { contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/WoodenShieldBase") });
            _propIcons.Add("EmptyPropIcon", new[] { contentManager.Load<Texture2D>("Sprites/ui/EmptyPropIcon") });
        }

        public SpriteFont GetHintTitleFont() => _hintTitleFont ?? throw new InvalidOperationException();
    }
}