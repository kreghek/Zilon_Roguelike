using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Players;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal class PersonMarkersPanel
    {
        private readonly IPlayer _player;
        private readonly int _positionOffsetY;
        private readonly SectorViewModelContext _sectorViewModelContext;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly IList<ActorViewModel> _visibleActors;
        private readonly IList<Marker> _drawnItemList;

        public PersonMarkersPanel(int positionOffsetY, IUiContentStorage uiContentStorage,
            SectorViewModelContext sectorViewModelContext,
            IPlayer player)
        {
            _positionOffsetY = positionOffsetY;
            _uiContentStorage = uiContentStorage;
            _sectorViewModelContext = sectorViewModelContext;
            _player = player;
            _visibleActors = new List<ActorViewModel>();
            _drawnItemList = new List<Marker>();
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            const int MARKER_WIDTH = 32;
            const int MARKER_HEGHT = 32;
            var index = 0;

            var viewPortHalfWidth = graphicsDevice.Viewport.Width / 2;
            var viewPortHeight = graphicsDevice.Viewport.Height;

            _drawnItemList.Clear();
            foreach (var item in _visibleActors)
            {
                var rect = new Rectangle(
                    index * MARKER_WIDTH + viewPortHalfWidth,
                    viewPortHeight - _positionOffsetY - MARKER_HEGHT,
                    MARKER_WIDTH,
                    MARKER_HEGHT
                );

                spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), rect, Color.White);

                _drawnItemList.Add(new Marker(rect, item));

                index++;
            }
        }

        public void Update()
        {
            _visibleActors.Clear();

            var actorViewModels = _sectorViewModelContext.GameObjects.OfType<ActorViewModel>().ToArray();
            foreach (var actorViewModel in actorViewModels)
            {
                if (actorViewModel.Actor.Person != _player.MainPerson
                    && actorViewModel.CanDraw)
                {
                    _visibleActors.Add(actorViewModel);
                }
            }

            CheckActorUnderHover();
        }

        private void CheckActorUnderHover()
        {
            const int MARKER_WIDTH = 32;
            const int MARKER_HEGHT = 32;

            var mouse = Mouse.GetState();
            var mouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);

            foreach (var item in _drawnItemList)
            {
                item.ActorViewModel.IsGraphicsOutlined = item.Rect.Intersects(mouseRect);
            }
        }

        private void HandleMarker(Marker item)
        {
            
        }

        private record Marker(Rectangle Rect, ActorViewModel ActorViewModel);
    }
}