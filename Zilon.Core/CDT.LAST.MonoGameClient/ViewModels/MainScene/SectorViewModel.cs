using System;
using System.Linq;

using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.ActorInteractionEvents;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class SectorViewModel
    {
        private readonly Game _game;
        private readonly Camera _camera;
        private readonly CommandInput _commandInput;
        private readonly IActorInteractionBus _intarectionBus;

        private readonly MapViewModel _mapViewModel;
        private readonly IPersonSoundContentStorage _personSoundContentStorage;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ISectorUiState _uiState;
        private readonly SectorViewModelContext _viewModelContext;

        public SectorViewModel(Game game, Camera camera, SpriteBatch spriteBatch)
        {
            _game = game;
            _camera = camera;
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)game).ServiceProvider;

            _player = serviceScope.GetRequiredService<IPlayer>();
            _uiState = serviceScope.GetRequiredService<ISectorUiState>();

            _intarectionBus = serviceScope.GetRequiredService<IActorInteractionBus>();

            _intarectionBus.NewEvent += IntarectionBus_NewEvent;

            var personVisualizationContentStorage =
                serviceScope.GetRequiredService<IPersonVisualizationContentStorage>();
            _personSoundContentStorage = serviceScope.GetRequiredService<IPersonSoundContentStorage>();

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

            _viewModelContext = new SectorViewModelContext(sector);

            foreach (var actor in Sector.ActorManager.Items)
            {
                var actorViewModel = new ActorViewModel(
                    game,
                    actor,
                    _viewModelContext,
                    personVisualizationContentStorage,
                    _personSoundContentStorage,
                    _spriteBatch);

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

            Sector.StaticObjectManager.Added += StaticObjectManager_Added;
            Sector.StaticObjectManager.Removed += StaticObjectManager_Removed;
            Sector.ActorManager.Removed += ActorManager_Removed;
            _uiState.ActiveActor.Actor.OpenedContainer += Actor_OpenedContainer;

            var commandFactory = new ServiceProviderCommandFactory(((LivGame)game).ServiceProvider);

            var commandPool = serviceScope.GetRequiredService<ICommandPool>();
            var commandInput = new CommandInput(_uiState, commandPool, _camera, Sector, _viewModelContext, commandFactory);
            _commandInput = commandInput;
        }

        private void Actor_OpenedContainer(object? sender, OpenContainerEventArgs e)
        {
            
        }

        private void StaticObjectManager_Removed(object? sender, ManagerItemsChangedEventArgs<IStaticObject> e)
        {
            foreach (var staticObject in e.Items)
            {
                var staticObjectViewModel = _viewModelContext.GameObjects.OfType<IContainerViewModel>().Single(x => x.StaticObject == staticObject);
                _viewModelContext.GameObjects.Remove((GameObjectBase)staticObjectViewModel);
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

        public void UnsubscribeEventHandlers()
        {
            _intarectionBus.NewEvent -= IntarectionBus_NewEvent;
            Sector.ActorManager.Removed -= ActorManager_Removed;
            Sector.StaticObjectManager.Added -= StaticObjectManager_Added;
            Sector.StaticObjectManager.Removed -= StaticObjectManager_Removed;
            _uiState.ActiveActor.Actor.OpenedContainer -= Actor_OpenedContainer;

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
                gameObject.CanDraw = true;

                var fowNode = visibleNodesMaterializedList.SingleOrDefault(x => x.Node == gameObject.Node);

                if (fowNode is null)
                {
                    gameObject.CanDraw = false;
                }

                if (fowNode != null && fowNode.State != SectorMapNodeFowState.Observing && gameObject.HiddenByFow)
                {
                    gameObject.CanDraw = false;
                }
                else if (fowNode != null && !gameObject.HiddenByFow)
                {
                    gameObject.UnderFog = fowNode.State != SectorMapNodeFowState.Observing;
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

        private void IntarectionBus_NewEvent(object? sender, ActorInteractionEventArgs e)
        {
            if (e.ActorInteractionEvent is DamageActorInteractionEvent damageActorInteractionEvent)
            {
                var actDescription = damageActorInteractionEvent.UsedActDescription;
                var targetPerson = damageActorInteractionEvent.TargetActor.Person;
                var soundEffect = _personSoundContentStorage.GetActHitSound(actDescription, targetPerson);
                soundEffect.CreateInstance().Play();
            }
        }
    }
}