using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal abstract class PropModalInventoryContextualMenuBase
    {
        private const int MENU_MARGIN = 5;
        private const int MENU_WIDTH = 128;
        private const int MENU_ITEM_HEIGHT = 16;

        protected readonly Point _position;
        protected readonly IUiContentStorage _uiContentStorage;

        protected readonly TextButton[] _menuItemButtons;
        protected readonly Point _size;

        public PropModalInventoryContextualMenuBase(Point position, IProp prop, IUiContentStorage uiContentStorage)
        {
            _position = new Point(position.X - MENU_MARGIN, position.Y - MENU_MARGIN);
            _position = position;
            _uiContentStorage = uiContentStorage;

            _menuItemButtons = InitItems(prop);

            var itemsHeight = _menuItemButtons.Length * MENU_ITEM_HEIGHT;
            _size = new Point(
                MENU_WIDTH + (MENU_MARGIN * 2),
                itemsHeight + (MENU_MARGIN * 2)
            );
        }


        public bool IsClosed { get; private set; }

        public bool IsCommandUsed { get; protected set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawBorder(spriteBatch);

            foreach (var button in _menuItemButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        public void Update()
        {
            foreach (var button in _menuItemButtons)
            {
                button.Update();
            }

            // Close menu if mouse is not on menu.

            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            var menuRect = new Rectangle(_position, _size);
            if (!mouseRect.Intersects(menuRect))
            {
                CloseMenu();
            }
        }

        protected void CloseMenu()
        {
            IsClosed = true;
        }

        protected abstract TextButton[] InitItems(IProp prop);

        private void DrawBorder(SpriteBatch spriteBatch)
        {
            // edges

            const int EDGE_SIZE = 5;
            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(_position, new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(0, 0, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + _size.X - EDGE_SIZE, _position.Y),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(7, 0, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(0, 7, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + _size.X - EDGE_SIZE, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(7, 7, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            // sides

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + EDGE_SIZE, _position.Y), new Point(_size.X - (EDGE_SIZE * 2), 6)),
                new Rectangle(EDGE_SIZE, 0, 2, 6),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + EDGE_SIZE, _position.Y + _size.Y - EDGE_SIZE),
                    new Point(_size.X - (EDGE_SIZE * 2), 6)),
                new Rectangle(EDGE_SIZE, 7, 2, 6),
                Color.White);
        }
    }
}