using System;
using System.Linq;

using CDT.LAST.MonoGameClient.Screens;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Commands.Sector;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.ActorInteractionEvents;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class CommandAdaptor
    {
        private readonly ICommandPool _commandPool;
        private readonly ServiceProviderCommandFactory _commandFactory;

        public CommandAdaptor(ICommandPool commandPool, ServiceProviderCommandFactory commandFactory)
        {
            _commandPool = commandPool;
            _commandFactory = commandFactory;
        }

        public void SwitchIntoCombatMode()
        {
            var switchIntoCombatCommand = _commandFactory.GetCommand<SwitchToCombatModeCommand>();

            var canExecuteResult = switchIntoCombatCommand.CanExecute();
            if (canExecuteResult.IsSuccess)
            {
                _commandPool.Push(switchIntoCombatCommand);
            }
        }
    }

    public sealed class SectorViewModel
    {
        private readonly Camera _camera;
        private readonly SectorInterator _sectorInterator;
        private readonly CommandAdaptor _commandAdaptor;
        private readonly GameObjectsViewModel _gameObjectsViewModel;
        private readonly IActorInteractionBus _intarectionBus;

        private readonly MapViewModel _mapViewModel;
        private readonly IPersonSoundContentStorage _personSoundContentStorage;
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

            var gameObjectParams = new GameObjectParams
            {
                Game = game,
                Camera = camera,
                UiState = _uiState,
                Player = _player,
                SpriteBatch = _spriteBatch,
                SectorViewModelContext = _viewModelContext,
                PersonSoundStorage = _personSoundContentStorage,
                PersonVisualizationContentStorage = personVisualizationContentStorage
            };
            _gameObjectsViewModel = new GameObjectsViewModel(gameObjectParams);

            var commandFactory = new ServiceProviderCommandFactory(((LivGame)game).ServiceProvider);

            var commandPool = serviceScope.GetRequiredService<ICommandPool>();
            var sectorInterator =
                new SectorInterator(_uiState, commandPool, _camera, Sector, _viewModelContext, commandFactory);
            _sectorInterator = sectorInterator;

            _commandAdaptor = new CommandAdaptor(commandPool, commandFactory);
        }

        public ISector Sector { get; }

        public void Draw(GameTime gameTime)
        {
            _mapViewModel.Draw(_camera.Transform);

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            _gameObjectsViewModel.Draw(gameTime);

            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            foreach (var visualEffect in _viewModelContext.EffectManager.VisualEffects.ToArray())
            {
                visualEffect.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }

        internal void SwitchCurrentPersonIntoCombatMode()
        {
            _commandAdaptor.SwitchIntoCombatMode();
        }

        public void UnsubscribeEventHandlers()
        {
            _intarectionBus.NewEvent -= IntarectionBus_NewEvent;
            _gameObjectsViewModel.UnsubscribeEventHandlers();
        }

        public void Update(GameTime gameTime)
        {
            var mainPerson = _player.MainPerson;
            if (mainPerson is null)
            {
                throw new InvalidOperationException();
            }

            _mapViewModel.Update(gameTime);

            _gameObjectsViewModel.Update(gameTime);

            foreach (var hitEffect in _viewModelContext.EffectManager.VisualEffects.ToArray())
            {
                hitEffect.Update(gameTime);

                if (hitEffect.IsComplete)
                {
                    _viewModelContext.EffectManager.VisualEffects.Remove(hitEffect);
                }
            }

            _sectorInterator.Update(_viewModelContext);
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