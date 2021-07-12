using System;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.ViewModels.MainScene;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;
using Zilon.Core.Commands;
using Zilon.Core.Scoring;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class MainScreen : GameSceneBase
    {
        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly ICommandPool _commandPool;
        private readonly ServiceProviderCommandFactory _commandFactory;
        private readonly BottomMenuPanel _bottomMenu;
        private readonly Camera _camera;
        private readonly ContainerModalDialog _containerModal;
        private readonly PersonConditionsPanel _personEffectsPanel;
        private readonly ModalDialogBase _personEquipmentModal;
        private readonly PersonStatsModalDialog _personStatsModal;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ITransitionPool _transitionPool;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;

        private ISector? _currentSector;

        private bool _isTransitionPerforming;
        private PersonMarkersPanel? _personMarkerPanel;

        private SectorViewModel? _sectorViewModel;

        public MainScreen(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
            _player = serviceScope.GetRequiredService<IPlayer>();
            _transitionPool = serviceScope.GetRequiredService<ITransitionPool>();
            _animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();
            _commandPool = serviceScope.GetRequiredService<ICommandPool>();
            _commandFactory = new ServiceProviderCommandFactory(((LivGame)game).ServiceProvider);

            _uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();

            _camera = new Camera();
            _personEffectsPanel =
                new PersonConditionsPanel(_uiState, screenX: 8, screenY: 8, uiContentStorage: _uiContentStorage);

            _personEquipmentModal = new PersonPropsModalDialog(
                _uiContentStorage,
                game.GraphicsDevice,
                _uiState,
                ((LivGame)game).ServiceProvider);

            _personStatsModal = new PersonStatsModalDialog(
                _uiContentStorage,
                game.GraphicsDevice,
                _uiState);

            _containerModal = new ContainerModalDialog(
                _uiState,
                _uiContentStorage,
                Game.GraphicsDevice,
                serviceScope);

            var humanActorTaskSource =
                serviceScope.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var mainPerson = _player.MainPerson;
            if (mainPerson is null)
            {
                throw new InvalidOperationException("Main person is not initalized. Generate globe first.");
            }

            _bottomMenu = new BottomMenuPanel(
                humanActorTaskSource,
                mainPerson.GetModule<ICombatActModule>(),
                _uiContentStorage,
                mainPerson.GetModule<IEquipmentModule>(),
                _uiState);
            _bottomMenu.PropButtonClicked += BottomMenu_PropButtonClicked;
            _bottomMenu.StatButtonClicked += BottomMenu_StatButtonClicked;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_sectorViewModel != null)
            {
                _sectorViewModel.Draw(gameTime);
            }

            DrawHud();

            DrawModals();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var visibleModal = CheckModalsIsVisible();
            if (visibleModal != null)
            {
                visibleModal.Update();
                return;
            }

            if (_sectorViewModel is null)
            {
                _sectorViewModel = new SectorViewModel(Game, _camera, _spriteBatch);
                _currentSector = _sectorViewModel.Sector;
                AddActiveActorEventHandling();

                // 32 + 8 == BottomPanel.PANEL_HEIGHT
                _personMarkerPanel =
                    new PersonMarkersPanel(32 + 8, _uiContentStorage, _sectorViewModel.ViewModelContext, _player, _uiState, _commandPool, _commandFactory);
            }

            if (!_isTransitionPerforming)
            {
                _sectorViewModel.Update(gameTime);
            }

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var isInTransition = _transitionPool.CheckPersonInTransition(_player.MainPerson);

            if (_uiState.ActiveActor != null && !isInTransition)
            {
                _bottomMenu.Update();

                if (_personMarkerPanel is not null)
                {
                    _personMarkerPanel.Update();
                }

                HandleMainUpdate(_uiState.ActiveActor);
            }
            else
            {
                HandleTransition(isInTransition);
            }
        }

        private void Actor_OpenedContainer(object? sender, OpenContainerEventArgs e)
        {
            _containerModal.Init(e.Container);
            _containerModal.Show();
        }

        private void AddActiveActorEventHandling()
        {
            if (_uiState.ActiveActor is not null)
            {
                _uiState.ActiveActor.Actor.OpenedContainer += Actor_OpenedContainer;
            }
        }

        private void BottomMenu_PropButtonClicked(object? sender, EventArgs e)
        {
            _personEquipmentModal.Show();
        }

        private void BottomMenu_StatButtonClicked(object? sender, EventArgs e)
        {
            _personStatsModal.Show();
        }

        private ModalDialogBase? CheckModalsIsVisible()
        {
            if (_personEquipmentModal.IsVisible)
            {
                return _personEquipmentModal;
            }

            if (_personStatsModal.IsVisible)
            {
                return _personStatsModal;
            }

            if (_containerModal.IsVisible)
            {
                return _containerModal;
            }

            return null;
        }

        private void DrawHud()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _personEffectsPanel.Draw(_spriteBatch);

            DrawPersonModePanel();

            if (_personMarkerPanel is not null)
            {
                _personMarkerPanel.Draw(_spriteBatch, Game.GraphicsDevice);
            }

            _spriteBatch.End();
        }

        private void DrawModals()
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (_personEquipmentModal.IsVisible)
            {
                _personEquipmentModal.Draw(_spriteBatch);
            }

            if (_personStatsModal.IsVisible)
            {
                _personStatsModal.Draw(_spriteBatch);
            }

            if (_containerModal.IsVisible)
            {
                _containerModal.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }

        private void DrawPersonModePanel()
        {
            var mainPerson = _player.MainPerson;
            if (mainPerson is null)
            {
                // Do not draw anything if main person is not initialized.
                Debug.Fail("This screen can't be constructed before globe generation screen.");
                return;
            }

            _bottomMenu.Draw(_spriteBatch, Game.GraphicsDevice);
        }

        private static ISectorNode? GetPlayerSectorNode(IPlayer player)
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
                    select sectorNode).SingleOrDefault();
        }

        private void HandleMainUpdate(IActorViewModel activeActor)
        {
            var sectorNodeWithPlayerPerson = GetPlayerSectorNode(_player);

            if (sectorNodeWithPlayerPerson != null)
            {
                var sectorWithPlayerPerson = sectorNodeWithPlayerPerson.Sector;
                UpdateCurrentSectorOrPerformTransition(sectorWithPlayerPerson, activeActor);
            }
            else
            {
                // This means the player person is dead (don't exists in any sector).
                // Or some error occured.
                if (activeActor.Actor.Person.CheckIsDead())
                {
                    _isTransitionPerforming = true;

                    HandleScreenChanging();

                    TargetScene = new ScoresScreen(Game, _spriteBatch);
                }
                else
                {
                    Debug.Fail("Main screen must load only if the player person is in any sector node.");
                }
            }
        }

        private void HandleScreenChanging()
        {
            _animationBlockerService.DropBlockers();

            if (_sectorViewModel is not null)
            {
                _sectorViewModel.UnsubscribeEventHandlers();
            }
            else
            {
                Debug.Fail("Sector view model must initalized before user performs transition and change screen.");
            }

            if (_uiState.ActiveActor is not null)
            {
                _uiState.ActiveActor.Actor.OpenedContainer -= Actor_OpenedContainer;
            }

            _bottomMenu.UnsubscribeEvents();

            _uiState.ActiveActor = null;
            _uiState.SelectedViewModel = null;
        }

        private void HandleTransition(bool isInTransition)
        {
            if (isInTransition)
            {
                if (!_isTransitionPerforming)
                {
                    LoadTransitionScreen();
                }
            }
            else if (_uiState.ActiveActor is null)
            {
                Debug.Fail("Main screen must load only after active actor was assigned.");
            }
            else
            {
                Debug.Fail("Unknown state.");
            }
        }

        private void LoadTransitionScreen()
        {
            HandleScreenChanging();

            _isTransitionPerforming = true;
            TargetScene = new TransitionScreen(Game, _spriteBatch);
        }

        private void UpdateCurrentSectorOrPerformTransition(ISector? sectorWithPlayerPerson,
            IActorViewModel activeActorViewModel)
        {
            if (_currentSector == sectorWithPlayerPerson)
            {
                _camera.Follow(activeActorViewModel, Game);

                _personEffectsPanel.Update();

                var activePerson = activeActorViewModel.Actor.Person;
                var activePersonCombatActModule = activePerson.GetModule<ICombatActModule>();
                if (!activePersonCombatActModule.IsCombatMode)
                {
                    _bottomMenu.Update();
                }
            }
            else if (!_isTransitionPerforming)
            {
                LoadTransitionScreen();
            }
            else
            {
                Debug.Fail("Unkown situation.");
            }
        }
    }
}