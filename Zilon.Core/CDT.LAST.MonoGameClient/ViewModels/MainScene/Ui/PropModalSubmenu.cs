using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class PropModalSubmenu
    {
        private readonly Point _position;
        private readonly IProp _prop;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly TextButton _equipButton;

        public PropModalSubmenu(Point position, IProp prop, IUiContentStorage uiContentStorage)
        {
            _position = position;
            _prop = prop;
            _uiContentStorage = uiContentStorage;

            _equipButton = new TextButton("equip", _uiContentStorage.GetButtonTexture(), _uiContentStorage.GetButtonFont(), new Rectangle(_position.X, _position.Y + 16, 100, 32));
            _equipButton.OnClick += EquipButton_OnClick;
        }

        private void EquipButton_OnClick(object? sender, EventArgs e)
        {
            IsClosed = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), new Rectangle(_position, new Point(100, 64)), Color.White);
            _equipButton.Draw(spriteBatch);
        }

        public void Update()
        {
            _equipButton.Update();
        }

        public bool IsClosed { get; private set; }
    }
}