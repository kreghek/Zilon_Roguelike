using System;
using System.Collections.Generic;
using System.Linq;

using CDT.LIV.MonoGameClient.Scenes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class SectorViewModelContext
    {
        public SectorViewModelContext(EffectManager effectManager)
        {
            GameObjects = new List<GameObjectBase>();
            EffectManager = effectManager;
        }

        public List<GameObjectBase> GameObjects { get; }

        public EffectManager EffectManager { get; }
    }

    public sealed class SectorViewModel
    {
        private readonly Camera _camera;
        private readonly SpriteBatch _spriteBatch;
        private readonly IPlayer _player;
        private readonly ISectorUiState _uiState;
        private readonly Zilon.Core.Tactics.ISector _sector;

        private readonly MapViewModel _mapViewModel;
        private readonly CommandInput _commandInput;
        private readonly Texture2D _cursorTexture;

        private readonly SectorViewModelContext _viewModelContext;

        public Zilon.Core.Tactics.ISector Sector => _sector;

        public SectorViewModel(Game game, Camera camera, SpriteBatch spriteBatch)
        {
            _camera = camera;
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)game).ServiceProvider;

            _player = serviceScope.GetRequiredService<IPlayer>();
            _uiState = serviceScope.GetRequiredService<ISectorUiState>();

            var sector = GetPlayerSectorNode(_player).Sector;

            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            _sector = sector;

            var playerActor = (from actor in _sector.ActorManager.Items
                               where actor.Person == _player.MainPerson
                               select actor).SingleOrDefault();

            _mapViewModel = new MapViewModel(game, _player, _uiState, _sector, spriteBatch);

            _viewModelContext = new SectorViewModelContext(new EffectManager());

            foreach (var actor in _sector.ActorManager.Items)
            {
                var actorViewModel = new ActorViewModel(game, actor, _viewModelContext, _spriteBatch);

                if (actor.Person == _player.MainPerson)
                {
                    _uiState.ActiveActor = actorViewModel;
                }

                _viewModelContext.GameObjects.Add(actorViewModel);
            }

            foreach (var staticObject in _sector.StaticObjectManager.Items)
            {
                var staticObjectModel = new StaticObjectViewModel(game, staticObject, _spriteBatch);

                _viewModelContext.GameObjects.Add(staticObjectModel);
            }

            _sector.ActorManager.Removed += ActorManager_Removed;

            var commandFactory = new ServiceProviderCommandFactory(((LivGame)game).ServiceProvider);

            var commandPool = serviceScope.GetRequiredService<ICommandPool>();
            var commandInput = new CommandInput(_uiState, commandPool, _camera, _sector, commandFactory);
            _commandInput = commandInput;

            _cursorTexture = game.Content.Load<Texture2D>("Sprites/ui/walk-cursor");
        }

        private void ActorManager_Removed(object? sender, Zilon.Core.Tactics.ManagerItemsChangedEventArgs<Zilon.Core.Tactics.IActor> e)
        {
            _viewModelContext.GameObjects.RemoveAll(x => x is IActorViewModel viewModel && e.Items.Contains(viewModel.Actor));
        }

        public void Draw(GameTime gameTime)
        {
            _mapViewModel.Draw(_camera.Transform);

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var fowData = _player.MainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(_sector);

            if (visibleFowNodeData is null)
            {
                throw new InvalidOperationException();
            }

            var gameObjectsMaterialized = _viewModelContext.GameObjects.OrderBy(x => ((HexNode)x.Node).OffsetCoords.Y).ToArray();
            var visibleNodesMaterializedList = visibleFowNodeData.Nodes.ToArray();
            foreach (var gameObject in gameObjectsMaterialized)
            {
                var fowNode = visibleNodesMaterializedList.SingleOrDefault(x => x.Node == gameObject.Node);

                if (fowNode is null)
                {
                    continue;
                }

                if (fowNode.State != Zilon.Core.Tactics.SectorMapNodeFowState.Observing && gameObject.HiddenByFow)
                {
                    continue;
                }

                gameObject.Draw(gameTime, _camera.Transform);
            }

            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            foreach (var hitEffect in _viewModelContext.EffectManager.HitEffects.ToArray())
            {
                hitEffect.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            // Print mouse position and draw cursor itself

            var mouseState = Mouse.GetState();
            _spriteBatch.Begin();
            _spriteBatch.Draw(_cursorTexture, new Vector2(mouseState.X, mouseState.Y), Color.White);
            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var fowData = _player.MainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(_sector);

            _mapViewModel.Update(gameTime);

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

                if (fowNode != null && fowNode.State != Zilon.Core.Tactics.SectorMapNodeFowState.Observing && gameObject.HiddenByFow)
                {
                    gameObject.Visible = false;
                }

                gameObject.Update(gameTime);
            }

            foreach (var hitEffect in _viewModelContext.EffectManager.HitEffects.ToArray())
            {
                hitEffect.Update(gameTime);

                if (hitEffect.IsComplete)
                {
                    _viewModelContext.EffectManager.HitEffects.Remove(hitEffect);
                }
            }

            _commandInput.Update(_viewModelContext);
        }

        private static ISectorNode GetPlayerSectorNode(IPlayer player)
        {
            if (player.Globe is null)
            {
                throw new InvalidOperationException();
            }

            return (from sectorNode in player.Globe.SectorNodes
                    let sector = sectorNode.Sector
                    where sector != null
                    from actor in sector.ActorManager.Items
                    where actor.Person == player.MainPerson
                    select sectorNode).Single();
        }
    }
}
