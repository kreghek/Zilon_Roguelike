using System;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.ViewModels.MainScene;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class MainScreen : GameSceneBase
    {
        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly IconButton _autoplayModeButton;
        private readonly Camera _camera;
        private readonly ContainerModalDialog _containerModal;
        private readonly PersonConditionsPanel _personEffectsPanel;
        private readonly IconButton _personEquipmentButton;
        private readonly ModalDialogBase _personEquipmentModal;
        private readonly IconButton _personStatsButton;
        private readonly PersonStatsModalDialog _personStatsModal;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ITransitionPool _transitionPool;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;
        private bool _autoplayHintIsShown;
        private string _autoplayModeButtonTitle;
        private ISector? _currentSector;

        private bool _isTransitionPerforming;

        private SectorViewModel? _sectorViewModel;

        public MainScreen(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
            _player = serviceScope.GetRequiredService<IPlayer>();
            _transitionPool = serviceScope.GetRequiredService<ITransitionPool>();
            _animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

            _camera = new Camera();
            _personEffectsPanel = new PersonConditionsPanel(game, _uiState, screenX: 8, screenY: 8);

            var uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();
            _uiContentStorage = uiContentStorage;

            var halfOfScreenX = game.GraphicsDevice.Viewport.Width / 2;
            var bottomOfScreenY = game.GraphicsDevice.Viewport.Height;
            _autoplayModeButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(0, 0, 16, 32)
                ),
                rect: new Rectangle(halfOfScreenX - 16, bottomOfScreenY - 32, 16, 32)
            );
            _autoplayModeButton.OnClick += AutoplayModeButton_OnClick;
            _autoplayModeButtonTitle = string.Format(UiResources.SwitchAutomodeButtonTitle,
                UiResources.SwitchAutomodeButtonOffTitle);

            _personEquipmentButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(16, 0, 16, 32)
                ),
                rect: new Rectangle(halfOfScreenX - 16 + 16, bottomOfScreenY - 32, 16, 32));
            _personEquipmentButton.OnClick += PersonEquipmentButton_OnClick;

            _personStatsButton = new IconButton(
                texture: uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                iconData: new IconData(
                    uiContentStorage.GetSmallVerticalButtonIconsTexture(),
                    new Rectangle(0, 32, 16, 32)
                ),
                rect: new Rectangle(halfOfScreenX - 16 + (16 * 2), bottomOfScreenY - 32, 16, 32));
            _personStatsButton.OnClick += PersonStatsButton_OnClick;

            _personEquipmentModal = new PersonPropsModalDialog(
                uiContentStorage,
                game.GraphicsDevice,
                _uiState,
                ((LivGame)game).ServiceProvider);

            _personStatsModal = new PersonStatsModalDialog(
                uiContentStorage,
                game.GraphicsDevice,
                _uiState);

            _containerModal = new ContainerModalDialog(
                _uiState,
                uiContentStorage,
                Game.GraphicsDevice,
                ((LivGame)game).ServiceProvider);
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
                var sectorNodeWithPlayerPerson = GetPlayerSectorNode(_player);

                if (sectorNodeWithPlayerPerson != null)
                {
                    var sectorWithPlayerPerson = sectorNodeWithPlayerPerson.Sector;
                    UpdateCurrentSectorOrPerformTransition(sectorWithPlayerPerson, _uiState.ActiveActor);
                }
                else
                {
                    // This means the player person is dead (don't exists in any sector).
                    // Or some error occured.
                    if (_uiState.ActiveActor.Actor.Person.CheckIsDead())
                    {
                        // Do nothing.
                        // In the near future there the scores screen will load.
                    }
                    else
                    {
                        Debug.Fail("Main screen must load only if the player person is in any sector node.");
                    }
                }
            }
            else
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

        private void AutoplayModeButton_OnClick(object? sender, EventArgs e)
        {
            var serviceScope = ((LivGame)Game).ServiceProvider;

            var humanTaskSource = serviceScope.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            if (humanTaskSource is IActorTaskControlSwitcher controlSwitcher)
            {
                switch (controlSwitcher.CurrentControl)
                {
                    case ActorTaskSourceControl.Human:
                        controlSwitcher.Switch(ActorTaskSourceControl.Bot);
                        _autoplayModeButtonTitle = string.Format(UiResources.SwitchAutomodeButtonTitle,
                            UiResources.SwitchAutomodeButtonOnTitle);
                        break;

                    case ActorTaskSourceControl.Bot:
                        controlSwitcher.Switch(ActorTaskSourceControl.Human);
                        _autoplayModeButtonTitle = string.Format(UiResources.SwitchAutomodeButtonTitle,
                            UiResources.SwitchAutomodeButtonOffTitle);
                        break;

                    default:
                        throw new InvalidOperationException(
                            "Unknown actor task control {controlSwitcher.CurrentControl}.");
                }
            }
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

        private void DetectAutoplayHint()
        {
            var halfOfScreenX = Game.GraphicsDevice.Viewport.Width / 2;
            var bottomOfScreenY = Game.GraphicsDevice.Viewport.Height;

            var autoplayButtonRect = new Rectangle(halfOfScreenX - 16, bottomOfScreenY - 32, 16, 32);

            var mouseState = Mouse.GetState();
            var mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            _autoplayHintIsShown = autoplayButtonRect.Intersects(mouseRect);
        }

        private void DrawHud()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _personEffectsPanel.Draw(_spriteBatch);

            _autoplayModeButton.Draw(_spriteBatch);
            if (_autoplayHintIsShown)
            {
                var titleTextSizeVector = _uiContentStorage.GetHintTitleFont().MeasureString(_autoplayModeButtonTitle);

                const int HINT_TEXT_SPACING = 8;

                var halfOfScreenX = Game.GraphicsDevice.Viewport.Width / 2;
                var bottomOfScreenY = Game.GraphicsDevice.Viewport.Height;
                var autoplayButtonRect = new Rectangle(halfOfScreenX - 16, bottomOfScreenY - 32, 16, 32);

                var hintRectangle = new Rectangle(
                    autoplayButtonRect.Left,
                    autoplayButtonRect.Top - (int)titleTextSizeVector.Y - (HINT_TEXT_SPACING * 2),
                    (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                    (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

                _spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

                _spriteBatch.DrawString(_uiContentStorage.GetHintTitleFont(),
                    _autoplayModeButtonTitle,
                    new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                    Color.Wheat);
            }

            _personEquipmentButton.Draw(_spriteBatch);
            _personStatsButton.Draw(_spriteBatch);

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
        }

        private void LoadTransitionScreen()
        {
            HandleScreenChanging();

            _isTransitionPerforming = true;
            TargetScene = new TransitionScreen(Game, _spriteBatch);
        }

        private void PersonEquipmentButton_OnClick(object? sender, EventArgs e)
        {
            _personEquipmentModal.Show();
        }

        private void PersonStatsButton_OnClick(object? sender, EventArgs e)
        {
            _personStatsModal.Show();
        }

        private void UpdateCurrentSectorOrPerformTransition(ISector? sectorWithPlayerPerson,
            IActorViewModel activeActorViewModel)
        {
            if (_currentSector == sectorWithPlayerPerson)
            {
                _camera.Follow(activeActorViewModel, Game);

                _personEffectsPanel.Update();

                _autoplayModeButton.Update();

                _personEquipmentButton.Update();
                _personStatsButton.Update();

                DetectAutoplayHint();
            }
            else if (!_isTransitionPerforming)
            {
                LoadTransitionScreen();
            }
        }
    }
}