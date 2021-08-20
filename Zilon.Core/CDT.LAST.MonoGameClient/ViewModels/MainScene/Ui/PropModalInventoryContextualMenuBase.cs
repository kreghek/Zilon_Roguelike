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
        protected const int MENU_MARGIN = 5;
        protected const int MENU_WIDTH = 128;
        protected const int MENU_ITEM_HEIGHT = 16;

        protected readonly Point _position;
        protected readonly IUiContentStorage _uiContentStorage;

        protected TextButton[]? _menuItemButtons;
        protected Point? _size;

        protected PropModalInventoryContextualMenuBase(Point position, IUiContentStorage uiContentStorage)
        {
            _position = new Point(position.X - MENU_MARGIN, position.Y - MENU_MARGIN);
            _position = position;
            _uiContentStorage = uiContentStorage;
        }

        public bool IsClosed { get; private set; }

        public bool IsCommandUsed { get; protected set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_menuItemButtons is null)
            {
                return;
            }

            if (_size is null)
            {
                return;
            }

            DrawBorder(spriteBatch, _size.Value);

            foreach (var button in _menuItemButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        public void Init(IProp prop)
        {
            _menuItemButtons = InitItems(prop);

            var itemsHeight = _menuItemButtons.Length * MENU_ITEM_HEIGHT;
            _size = new Point(
                MENU_WIDTH + (MENU_MARGIN * 2),
                itemsHeight + (MENU_MARGIN * 2)
            );
        }

        public void Update()
        {
            if (_menuItemButtons is null)
            {
                return;
            }

            foreach (var button in _menuItemButtons)
            {
                button.Update();
            }

            CloseMenuIfAnyMouseButtonPressed();
        }

        private void CloseMenuIfAnyMouseButtonPressed()
        {
            if (_size == null)
                return;

            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            var menuRect = new Rectangle(_position, _size.Value);
            var wasPressedAnyMouseButton = mouseState.LeftButton == ButtonState.Pressed ||
                                           mouseState.RightButton == ButtonState.Pressed ||
                                           mouseState.MiddleButton == ButtonState.Pressed;
            var isClickMenuOut = !mouseRect.Intersects(menuRect);
            if (isClickMenuOut && wasPressedAnyMouseButton)
            {
                CloseMenu();
            }
        }

        protected void CloseMenu()
        {
            IsClosed = true;
        }

        protected abstract TextButton[] InitItems(IProp prop);

        private void DrawBorder(SpriteBatch spriteBatch, Point size)
        {
            // edges

            const int EDGE_SIZE = 5;
            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(_position, new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(0, 0, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + size.X - EDGE_SIZE, _position.Y),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(7, 0, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X, _position.Y + size.Y - EDGE_SIZE),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(0, 7, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + size.X - EDGE_SIZE, _position.Y + size.Y - EDGE_SIZE),
                    new Point(EDGE_SIZE, EDGE_SIZE)),
                new Rectangle(7, 7, EDGE_SIZE, EDGE_SIZE),
                Color.White);

            // sides

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + EDGE_SIZE, _position.Y), new Point(size.X - (EDGE_SIZE * 2), 6)),
                new Rectangle(EDGE_SIZE, 0, 2, 6),
                Color.White);

            spriteBatch.Draw(_uiContentStorage.GetContextualMenuBorderTexture(),
                new Rectangle(new Point(_position.X + EDGE_SIZE, _position.Y + size.Y - EDGE_SIZE),
                    new Point(size.X - (EDGE_SIZE * 2), 6)),
                new Rectangle(EDGE_SIZE, 7, 2, 6),
                Color.White);
        }
    }
}