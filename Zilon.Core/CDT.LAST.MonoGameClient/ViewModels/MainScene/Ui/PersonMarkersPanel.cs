using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public PersonMarkersPanel(int positionOffsetY, IUiContentStorage uiContentStorage,
            SectorViewModelContext sectorViewModelContext,
            IPlayer player)
        {
            _positionOffsetY = positionOffsetY;
            _uiContentStorage = uiContentStorage;
            _sectorViewModelContext = sectorViewModelContext;
            _player = player;
            _visibleActors = new List<ActorViewModel>();
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            const int MARKER_WIDTH = 32;
            const int MARKER_HEGHT = 32;
            var index = 0;

            var viewPortHalfWidth = graphicsDevice.Viewport.Width / 2;
            var viewPortHeight = graphicsDevice.Viewport.Height;

            foreach (var item in _visibleActors)
            {
                spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), new Rectangle(
                    index * MARKER_WIDTH + viewPortHalfWidth,
                    viewPortHeight - _positionOffsetY - MARKER_HEGHT,
                    MARKER_WIDTH,
                    MARKER_HEGHT
                ), Color.White);
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
        }
    }
}