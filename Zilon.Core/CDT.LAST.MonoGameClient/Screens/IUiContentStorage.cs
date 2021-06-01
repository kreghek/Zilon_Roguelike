using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Screens
{
    public interface IUiContentStorage
    {
        Texture2D GetButtonTexture();

        SpriteFont GetButtonFont();

        Texture2D[] GetModalTopTextures();

        Texture2D[] GetModalBottomTextures();

        Texture2D GetModalShadowTexture();

        Texture2D GetPropIcon(string sid);

        void LoadContent(ContentManager contentManager);
    }

    sealed class UiContentStorage : IUiContentStorage
    {
        private Texture2D? _buttonTexture;
        private Texture2D[]? _modalTopTextures;
        private Texture2D[]? _modalBottomTextures;
        private Texture2D? _modalShadowTexture;
        private SpriteFont? _buttonFont;
        private readonly Dictionary<string, Texture2D> _propIcons;

        public UiContentStorage()
        {
            _propIcons = new Dictionary<string, Texture2D>();
        }

        public SpriteFont GetButtonFont() => _buttonFont ?? throw new InvalidOperationException();

        public Texture2D GetButtonTexture() => _buttonTexture ?? throw new InvalidOperationException();

        public Texture2D[] GetModalBottomTextures() => _modalBottomTextures ?? throw new InvalidOperationException();

        public Texture2D[] GetModalTopTextures() => _modalTopTextures ?? throw new InvalidOperationException();

        public Texture2D GetPropIcon(string sid)
        {
            return _propIcons["test"];
        }

        public Texture2D GetModalShadowTexture() => _modalShadowTexture ?? throw new InvalidOperationException();

        public void LoadContent(ContentManager contentManager)
        {
            _buttonFont = contentManager.Load<SpriteFont>("Fonts/Main");
            _buttonTexture = contentManager.Load<Texture2D>("Sprites/ui/button");
            _modalShadowTexture = contentManager.Load<Texture2D>("Sprites/ui/ModalDialogShadow");
            _modalTopTextures = new[] { contentManager.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundTop1") };
            _modalBottomTextures = new[] { contentManager.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundBottom1") };

            _propIcons.Add("test", contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/ShortSwordBase"));
        }
    }
}
