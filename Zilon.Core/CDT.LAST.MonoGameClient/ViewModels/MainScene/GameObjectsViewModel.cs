using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDT.LAST.MonoGameClient.Screens;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class GameObjectsViewModel
    {
        private readonly Camera _camera;
        private readonly Game _game;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ISectorUiState _uiState;
        private readonly SectorViewModelContext _viewModelContext;

        public GameObjectsViewModel(GameObjectParams gameObjectParams)
        {
            _viewModelContext = gameObjectParams.SectorViewModelContext ??
                                throw new ArgumentException(
                                    $"{nameof(gameObjectParams.SectorViewModelContext)} is not defined.",
                                    nameof(gameObjectParams));
            _player = gameObjectParams.Player ??
                      throw new ArgumentException($"{nameof(gameObjectParams.Player)} is not defined.",
                          nameof(gameObjectParams));
            _camera = gameObjectParams.Camera ??
                      throw new ArgumentException($"{nameof(gameObjectParams.Camera)} is not defined.",
                          nameof(gameObjectParams));
            _game = gameObjectParams.Game ??
                    throw new ArgumentException($"{nameof(gameObjectParams.Game)} is not defined.",
                        nameof(gameObjectParams));
            _spriteBatch = gameObjectParams.SpriteBatch ??
                           throw new ArgumentException($"{nameof(gameObjectParams.SpriteBatch)} is not defined.",
                               nameof(gameObjectParams));

            _uiState = gameObjectParams.UiState ?? throw new ArgumentException(
                $"{nameof(gameObjectParams.UiState)} is not defined.",
                nameof(gameObjectParams));

            foreach (var actor in _viewModelContext.Sector.ActorManager.Items)
            {
                var actorViewModel = new ActorViewModel(actor, gameObjectParams);

                if (actor.Person == _player.MainPerson)
                {
                    _uiState.ActiveActor = actorViewModel;
                }

                _viewModelContext.GameObjects.Add(actorViewModel);
            }

            foreach (var staticObject in _viewModelContext.Sector.StaticObjectManager.Items)
            {
                var staticObjectModel =
                    new StaticObjectViewModel(gameObjectParams.Game, staticObject, gameObjectParams.SpriteBatch);

                _viewModelContext.GameObjects.Add(staticObjectModel);
            }

            var sector = _viewModelContext.Sector;
            sector.ActorManager.Removed += ActorManager_Removed;
            sector.StaticObjectManager.Added += StaticObjectManager_Added;
            sector.StaticObjectManager.Removed += StaticObjectManager_Removed;
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

            try
            {
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

                    var visualEffects = _viewModelContext.EffectManager.VisualEffects
                        .Where(x => x.BoundGameObjects.Contains(gameObject)).ToArray();
                    _spriteBatch.Begin(transformMatrix: _camera.Transform);

                    foreach (var visualEffect in visualEffects)
                    {
                        visualEffect.Draw(_spriteBatch, backing: true);
                    }

                    _spriteBatch.End();

                    gameObject.Draw(gameTime, _camera.Transform);

                    _spriteBatch.Begin(transformMatrix: _camera.Transform);

                    foreach (var visualEffect in visualEffects)
                    {
                        visualEffect.Draw(_spriteBatch, backing: false);
                    }

                    _spriteBatch.End();
                }
            }
            catch
            {
                // Smell. Handle multithread access to fow data.
                // Just ignore this frame and try to draw next correctly.
            }
        }

        public void UnsubscribeEventHandlers()
        {
            var sector = _viewModelContext.Sector;
            sector.ActorManager.Removed -= ActorManager_Removed;
            sector.StaticObjectManager.Added -= StaticObjectManager_Added;
            sector.StaticObjectManager.Removed -= StaticObjectManager_Removed;

            foreach (var gameObject in _viewModelContext.GameObjects)
            {
                switch (gameObject)
                {
                    case ActorViewModel actorViewModel:
                        actorViewModel.UnsubscribeEventHandlers();
                        break;

                    case StaticObjectViewModel staticObjectViewModel:
                        // Currently do nothing since staticObjectViewModel have no subscribtions.
                        break;

                    default:
                        Debug.Fail("Unknown type of game object.");
                        break;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            var mainPerson = _player.MainPerson;
            if (mainPerson is null)
            {
                return;
            }

            var fowData = mainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(_viewModelContext.Sector);

            var gameObjectsFixedList = _viewModelContext.GameObjects.ToArray();
            foreach (var gameObject in gameObjectsFixedList)
            {
                gameObject.CanDraw = true;

                var fowNode = visibleFowNodeData.GetFowByNode(gameObject.Node);

                if (fowNode is null)
                {
                    gameObject.CanDraw = false;
                }

                if (fowNode != null && fowNode.State != SectorMapNodeFowState.Observing && gameObject.HiddenByFow)
                {
                    gameObject.CanDraw = false;
                }

                if (gameObject.CanDraw)
                {
                    if (gameObject is ActorViewModel viewModel)
                    {
                        viewModel.IsGraphicsOutlined = ReferenceEquals(gameObject, _uiState.HoverViewModel);
                    }
                }

                gameObject.Update(gameTime);
            }
        }

        private void ActorManager_Removed(object? sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            var gameObjects = _viewModelContext.GameObjects
                .Where(x => x is IActorViewModel viewModel && e.Items.Contains(viewModel.Actor)).ToArray();

            foreach (var gameObject in gameObjects)
            {
                gameObject.HandleRemove();
                _viewModelContext.GameObjects.Remove(gameObject);
            }
        }

        private void StaticObjectManager_Added(object? sender, ManagerItemsChangedEventArgs<IStaticObject> e)
        {
            foreach (var staticObject in e.Items)
            {
                var staticObjectModel = new StaticObjectViewModel(_game, staticObject, _spriteBatch);

                _viewModelContext.GameObjects.Add(staticObjectModel);
            }
        }

        private void StaticObjectManager_Removed(object? sender, ManagerItemsChangedEventArgs<IStaticObject> e)
        {
            foreach (var staticObject in e.Items)
            {
                var staticObjectViewModel = _viewModelContext.GameObjects.OfType<IContainerViewModel>()
                    .Single(x => x.StaticObject == staticObject);
                _viewModelContext.GameObjects.Remove((GameObjectBase)staticObjectViewModel);
            }
        }
    }
}