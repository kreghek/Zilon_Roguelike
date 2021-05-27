using System;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.ViewModels.MainScene;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class MainScreen : GameSceneBase
    {
        private readonly Button _autoplayModeButton;
        private readonly Camera _camera;
        private readonly Button _personButton;
        private readonly PersonConditionsPanel _personEffectsPanel;
        private readonly ModalDialog _personModal;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ITransitionPool _transitionPool;
        private readonly ISectorUiState _uiState;
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

            _camera = new Camera();
            _personEffectsPanel = new PersonConditionsPanel(game, _uiState, screenX: 0, screenY: 0);

            var buttonTexture = game.Content.Load<Texture2D>("Sprites/ui/button");
            var buttonFont = game.Content.Load<SpriteFont>("Fonts/Main");

            var halfOfScreenX = game.GraphicsDevice.Viewport.Width / 2;
            var bottomOfScreenY = game.GraphicsDevice.Viewport.Height;
            _autoplayModeButton = new Button(
                string.Format(UiResources.SwitchAutomodeButtonTitle, UiResources.SwitchAutomodeButtonOffTitle),
                buttonTexture,
                buttonFont,
                new Rectangle(halfOfScreenX - 16, bottomOfScreenY - 32, 32, 32)
            );
            _autoplayModeButton.OnClick += AutoplayModeButton_OnClick;

            _personButton = new Button("p", buttonTexture, buttonFont,
                new Rectangle(halfOfScreenX - 16 + 32, bottomOfScreenY - 32, 32, 32));
            _personButton.OnClick += PersonButton_OnClick;

            var modalBackgroundTopTexture = game.Content.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundTop1");
            var modalBackgroundBottomTexture = game.Content.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundBottom1");
            var modalShadowTexture = game.Content.Load<Texture2D>("Sprites/ui/ModalDialogShadow");
            _personModal = new ModalDialog(modalBackgroundTopTexture, modalBackgroundBottomTexture, modalShadowTexture,
                buttonTexture, buttonFont,
                game.GraphicsDevice);
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
            }

            _sectorViewModel.Update(gameTime);

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var isInTransition = _transitionPool.CheckPersonInTransition(_player.MainPerson);

            if (_uiState.ActiveActor != null && !isInTransition)
            {
                var sectorNode = GetPlayerSectorNode(_player);

                if (sectorNode != null)
                {
                    if (_currentSector == sectorNode.Sector)
                    {
                        _camera.Follow(_uiState.ActiveActor, Game);

                        _personEffectsPanel.Update();

                        _autoplayModeButton.Update();

                        _personButton.Update();
                    }
                    else if (!_isTransitionPerforming)
                    {
                        _isTransitionPerforming = true;
                        TargetScene = new TransitionScreen(Game, _spriteBatch);
                    }
                }
            }
            else
            {
                if (!_isTransitionPerforming)
                {
                    _isTransitionPerforming = true;
                    TargetScene = new TransitionScreen(Game, _spriteBatch);
                }
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
                        _autoplayModeButton.Title = string.Format(UiResources.SwitchAutomodeButtonTitle,
                            UiResources.SwitchAutomodeButtonOnTitle);
                        break;

                    case ActorTaskSourceControl.Bot:
                        controlSwitcher.Switch(ActorTaskSourceControl.Human);
                        _autoplayModeButton.Title = string.Format(UiResources.SwitchAutomodeButtonTitle,
                            UiResources.SwitchAutomodeButtonOffTitle);
                        break;

                    default:
                        throw new InvalidOperationException(
                            "Unknown actor task control {controlSwitcher.CurrentControl}.");
                }
            }
        }

        private ModalDialog? CheckModalsIsVisible()
        {
            if (_personModal.IsVisible)
            {
                return _personModal;
            }

            return null;
        }

        private void DrawHud()
        {
            _spriteBatch.Begin();
            _personEffectsPanel.Draw(_spriteBatch);

            _autoplayModeButton.Draw(_spriteBatch);
            _personButton.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        private void DrawModals()
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (_personModal.IsVisible)
            {
                _personModal.Draw(_spriteBatch);
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

        private void PersonButton_OnClick(object? sender, EventArgs e)
        {
            _personModal.Show();
        }
    }
}