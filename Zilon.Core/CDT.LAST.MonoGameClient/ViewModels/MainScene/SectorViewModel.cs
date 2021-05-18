using System;
using System.Linq;

using CDT.LAST.MonoGameClient.Scenes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class SectorViewModel
    {
        private readonly Camera _camera;
        private readonly CommandInput _commandInput;

        private readonly MapViewModel _mapViewModel;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ISectorUiState _uiState;

        private readonly SectorViewModelContext _viewModelContext;

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

            Sector = sector;

            var playerActor = (from actor in Sector.ActorManager.Items
                               where actor.Person == _player.MainPerson
                               select actor).SingleOrDefault();

            _mapViewModel = new MapViewModel(game, _player, _uiState, Sector, spriteBatch);

            _viewModelContext = new SectorViewModelContext(new EffectManager());

            foreach (var actor in Sector.ActorManager.Items)
            {
                var actorViewModel = new ActorViewModel(game, actor, _viewModelContext, _spriteBatch);

                if (actor.Person == _player.MainPerson)
                {
                    _uiState.ActiveActor = actorViewModel;
                }

                _viewModelContext.GameObjects.Add(actorViewModel);
            }

            foreach (var staticObject in Sector.StaticObjectManager.Items)
            {
                var staticObjectModel = new StaticObjectViewModel(game, staticObject, _spriteBatch);

                _viewModelContext.GameObjects.Add(staticObjectModel);
            }

            Sector.ActorManager.Removed += ActorManager_Removed;

            var commandFactory = new ServiceProviderCommandFactory(((LivGame)game).ServiceProvider);

            var commandPool = serviceScope.GetRequiredService<ICommandPool>();
            var commandInput = new CommandInput(_uiState, commandPool, _camera, Sector, commandFactory);
            _commandInput = commandInput;
        }

        public ISector Sector { get; }

        public void Draw(GameTime gameTime)
        {
            _mapViewModel.Draw(_camera.Transform);

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var fowData = _player.MainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(Sector);

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

            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            foreach (var hitEffect in _viewModelContext.EffectManager.HitEffects.ToArray())
            {
                hitEffect.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var fowData = _player.MainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(Sector);

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

                if (fowNode != null && fowNode.State != SectorMapNodeFowState.Observing && gameObject.HiddenByFow)
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

        private void ActorManager_Removed(object? sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            _viewModelContext.GameObjects.RemoveAll(x =>
                x is IActorViewModel viewModel && e.Items.Contains(viewModel.Actor));
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