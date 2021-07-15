using System;
using System.Linq;

using CDT.LAST.MonoGameClient.Screens;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.ActorInteractionEvents;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class SectorViewModel
    {
        private readonly Camera _camera;
        private readonly CommandInput _commandInput;
        private readonly GameObjectsViewModel _gameObjectsViewModel;
        private readonly IActorInteractionBus _intarectionBus;

        private readonly MapViewModel _mapViewModel;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ISectorUiState _uiState;

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
            var personSoundContentStorage = serviceScope.GetRequiredService<IPersonSoundContentStorage>();
            var gameObjectVisualizationContentStorage =
                serviceScope.GetRequiredService<IGameObjectVisualizationContentStorage>();

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

            ViewModelContext = new SectorViewModelContext(sector);

            var gameObjectParams = new GameObjectParams
            {
                Game = game,
                Camera = camera,
                UiState = _uiState,
                Player = _player,
                SpriteBatch = _spriteBatch,
                SectorViewModelContext = ViewModelContext,
                PersonSoundStorage = personSoundContentStorage,
                PersonVisualizationContentStorage = personVisualizationContentStorage,
                GameObjectVisualizationContentStorage = gameObjectVisualizationContentStorage
            };
            _gameObjectsViewModel = new GameObjectsViewModel(gameObjectParams);

            var commandFactory = new ServiceProviderCommandFactory(((LivGame)game).ServiceProvider);

            var commandPool = serviceScope.GetRequiredService<ICommandPool>();
            var commandInput =
                new CommandInput(_uiState, commandPool, _camera, Sector, ViewModelContext, commandFactory);
            _commandInput = commandInput;
        }

        public ISector Sector { get; }

        public SectorViewModelContext ViewModelContext { get; }

        public void Draw(GameTime gameTime)
        {
            _mapViewModel.Draw(_camera.Transform);

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            DrawCorpses();

            _gameObjectsViewModel.Draw(gameTime);
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

            ViewModelContext.CorpseManager.Update(gameTime);

            foreach (var hitEffect in ViewModelContext.EffectManager.VisualEffects.ToArray())
            {
                hitEffect.Update(gameTime);

                if (hitEffect.IsComplete)
                {
                    ViewModelContext.EffectManager.VisualEffects.Remove(hitEffect);
                }
            }

            _commandInput.Update(ViewModelContext);
        }

        private void DrawCorpses()
        {
            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            ViewModelContext.CorpseManager.Draw(_spriteBatch);

            _spriteBatch.End();
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
                var targetActor = damageActorInteractionEvent.TargetActor;

                var attackerViewModel = ViewModelContext.GameObjects.OfType<ActorViewModel>()
                    .Single(x => x.Actor == damageActorInteractionEvent.Actor);
                if (attackerViewModel.CanDraw)
                {
                    attackerViewModel.RunCombatActUsageAnimation(actDescription, targetActor.Node);
                }

                var targetViewModel = ViewModelContext.GameObjects.OfType<ActorViewModel>()
                    .Single(x => x.Actor == targetActor);
                if (targetViewModel.CanDraw)
                {
                    var targetPersonIsStillAlive = !targetActor.Person.CheckIsDead();
                    if (targetPersonIsStillAlive)
                    {
                        targetViewModel.RunDamageReceivedAnimation(attackerViewModel.Node);
                    }
                }
            }
        }
    }
}