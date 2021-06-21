﻿using System;
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
    internal record GameObjectParams
    {
        public Camera Camera { get; init; }
        public Game Game { get; init; }
        public IPersonSoundContentStorage? PersonSoundStorage { get; internal set; }
        public IPersonVisualizationContentStorage PersonVisualizationContentStorage { get; internal set; }
        public IPlayer Player { get; init; }
        public SectorViewModelContext SectorViewModelContext { get; init; }
        public SpriteBatch SpriteBatch { get; init; }
        public ISectorUiState UiState { get; init; }
    }

    internal class GameObjectsViewModel
    {
        private const double UPDATE_DELAY_SECONDS = 0f;
        private readonly Camera _camera;
        private readonly Game _game;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly SectorViewModelContext _viewModelContext;
        private double _updateCounter;

        public GameObjectsViewModel(GameObjectParams gameObjectParams)
        {
            _viewModelContext = gameObjectParams.SectorViewModelContext;
            _player = gameObjectParams.Player;
            _camera = gameObjectParams.Camera;
            _game = gameObjectParams.Game;
            _spriteBatch = gameObjectParams.SpriteBatch;

            foreach (var actor in _viewModelContext.Sector.ActorManager.Items)
            {
                var actorViewModel = new ActorViewModel(actor, gameObjectParams);

                if (actor.Person == _player.MainPerson)
                {
                    gameObjectParams.UiState.ActiveActor = actorViewModel;
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
                }
            }
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

                gameObject.Update(gameTime);
            }
        }

        private void ActorManager_Removed(object? sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            _viewModelContext.GameObjects.RemoveAll(x =>
                x is IActorViewModel viewModel && e.Items.Contains(viewModel.Actor));
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