using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    class GameObjectsViewModel
    {
        private readonly SectorViewModelContext _viewModelContext;
        private readonly IPlayer _player;
        private readonly Camera _camera;
        private double _updateCounter;

        private const double UPDATE_DELAY_SECONDS = 1f;

        public GameObjectsViewModel(SectorViewModelContext viewModelContext, IPlayer player, Camera camera, SpriteBatch spriteBatch, Game game, ISectorUiState _uiState)
        {
            _viewModelContext = viewModelContext;
            _player = player;
            _camera = camera;


            foreach (var actor in viewModelContext.Sector.ActorManager.Items)
            {
                var actorViewModel = new ActorViewModel(game, actor, _viewModelContext, spriteBatch);

                if (actor.Person == _player.MainPerson)
                {
                    _uiState.ActiveActor = actorViewModel;
                }

                _viewModelContext.GameObjects.Add(actorViewModel);
            }

            foreach (var staticObject in viewModelContext.Sector.StaticObjectManager.Items)
            {
                var staticObjectModel = new StaticObjectViewModel(game, staticObject, spriteBatch);

                _viewModelContext.GameObjects.Add(staticObjectModel);
            }

            viewModelContext.Sector.ActorManager.Removed += ActorManager_Removed;
        }

        private void ActorManager_Removed(object? sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            _viewModelContext.GameObjects.RemoveAll(x =>
                x is IActorViewModel viewModel && e.Items.Contains(viewModel.Actor));
        }

        public void Update(GameTime gameTime)
        {
            _updateCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (_updateCounter < UPDATE_DELAY_SECONDS)
            {
                return;
            }

            _updateCounter = 0;

            var fowData = _player.MainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(_viewModelContext.Sector);

            var gameObjectsFixedList = _viewModelContext.GameObjects.ToArray();
            var visibleNodesMaterializedList = visibleFowNodeData.Nodes.ToArray();
            foreach (var gameObject in gameObjectsFixedList)
            {
                gameObject.Visible = true;

                var fowNode = visibleNodesMaterializedList.SingleOrDefault(x => x.Node == gameObject.Node);

                if (fowNode is null)
                {
                    gameObject.Visible = false;
                }

                if (fowNode != null && fowNode.State != SectorMapNodeFowState.Observing && gameObject.HiddenByFow)
                {
                    gameObject.Visible = false;
                }

                gameObject.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var fowData = _player.MainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(_viewModelContext.Sector);

            if (visibleFowNodeData is null)
            {
                throw new InvalidOperationException();
            }

            var gameObjectsMaterialized =
                _viewModelContext.GameObjects.OrderBy(x => ((HexNode)x.Node).OffsetCoords.Y).ToArray();
            var visibleNodesMaterializedList = visibleFowNodeData.Nodes.ToArray();
            foreach (var gameObject in gameObjectsMaterialized)
            {
                var fowNode = visibleNodesMaterializedList.SingleOrDefault(x => x.Node == gameObject.Node);

                if (fowNode is null)
                {
                    continue;
                }

                if (fowNode.State != SectorMapNodeFowState.Observing && gameObject.HiddenByFow)
                {
                    continue;
                }

                gameObject.Draw(gameTime, _camera.Transform);
            }
        }
    }
}
