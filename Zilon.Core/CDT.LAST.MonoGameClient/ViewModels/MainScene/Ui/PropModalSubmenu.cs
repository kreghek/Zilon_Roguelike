using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class PropModalSubmenu
    {
        private const int MENU_MARGIN = 5;
        private readonly TextButton _equipButton;
        private readonly Point _position;
        private readonly Point _size;
        private readonly IProp _prop;
        private readonly IUiContentStorage _uiContentStorage;

        public PropModalSubmenu(Point position, IProp prop, IUiContentStorage uiContentStorage)
        {
            _position = new Point(position.X - MENU_MARGIN, position.Y - MENU_MARGIN);
            _size = new Point(100, 64);
            _prop = prop;
            _uiContentStorage = uiContentStorage;

            _equipButton = new TextButton("equip", _uiContentStorage.GetButtonTexture(),
                _uiContentStorage.GetButtonFont(), new Rectangle(_position.X, _position.Y + 16, _size.X - (MENU_MARGIN * 2), 32));
            _equipButton.OnClick += EquipButton_OnClick;
        }

        public bool IsClosed { get; private set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), new Rectangle(_position, _size),
                Color.White);
            _equipButton.Draw(spriteBatch);
        }

        public void Update()
        {
            _equipButton.Update();

            // Close menu if mouse is not on menu.

            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            var menuRect = new Rectangle(_position, _size);
            if (!mouseRect.Intersects(menuRect))
            {
                IsClosed = true;
            }
        }

        private void EquipButton_OnClick(object? sender, EventArgs e)
        {
            IsClosed = true;
        }
    }
}