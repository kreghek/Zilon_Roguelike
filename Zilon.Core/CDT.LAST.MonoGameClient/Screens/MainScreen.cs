using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.GameComponents;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.ViewModels;
using CDT.LAST.MonoGameClient.ViewModels.MainScene;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class MainScreen : GameSceneBase
    {
        private const int STAT_ROW_HEIGHT = 16;
        private const int STAT_NUMS_X_POSITION = 32;

        private static bool _showControlTutorial = true;

        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly BottomMenuPanel _bottomMenu;
        private readonly Camera _camera;
        private readonly ServiceProviderCommandFactory _commandFactory;
        private readonly ICommandPool _commandPool;
        private readonly ContainerModalDialog _containerModal;
        private readonly IPlayerEventLogService _logService;
        private readonly PersonConditionsPanel _personEffectsPanel;
        private readonly ModalDialogBase _personEquipmentModal;
        private readonly PersonStatsModalDialog _personStatsModal;
        private readonly PersonTraitsModalDialog _personTraitsModal;
        private readonly IPlayer _player;
        private readonly IScoreManager _scoreManager;
        private readonly SpriteBatch _spriteBatch;
        private readonly ITransitionPool _transitionPool;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;
        private ISector? _currentSector;

        private bool _isBeginTransition;

        private bool _isTransitionPerforming;

        private KeyboardState? _lastKeyboardState;

        private double _loadingAnimCounter;
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
            var uiSoundStorage = serviceScope.GetRequiredService<IUiSoundStorage>();

            _uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();

            var soundtrackManager = serviceScope.GetRequiredService<SoundtrackManager>();

            _camera = new Camera();
            _personEffectsPanel =
                new PersonConditionsPanel(_uiState, screenX: game.GraphicsDevice.Viewport.Bounds.Center.X, screenY: 8,
                    _uiContentStorage, uiSoundStorage,
                    soundtrackManager, GraphicsDevice);

            _personEquipmentModal = new PersonPropsModalDialog(
                _uiContentStorage,
                game.GraphicsDevice,
                _uiState,
                ((LivGame)game).ServiceProvider);

            _personStatsModal = new PersonStatsModalDialog(
                _uiContentStorage,
                game.GraphicsDevice,
                _uiState);

            _personTraitsModal = new PersonTraitsModalDialog(
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

            _logService = serviceScope.GetRequiredService<IPlayerEventLogService>();

            _bottomMenu = new BottomMenuPanel(
                humanActorTaskSource,
                mainPerson.GetModule<ICombatActModule>(),
                _uiContentStorage,
                mainPerson.GetModule<IEquipmentModule>(),
                _uiState,
                _commandPool,
                _commandFactory,
                serviceScope.GetRequiredService<ICommandLoopContext>(),
                _logService);
            _bottomMenu.PropButtonClicked += BottomMenu_PropButtonClicked;
            _bottomMenu.StatButtonClicked += BottomMenu_StatButtonClicked;
            _bottomMenu.TraitsButtonClicked += BottomMenu_TraitsButtonClicked;

            _scoreManager = serviceScope.GetRequiredService<IScoreManager>();
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

            if (_isBeginTransition)
            {
                _loadingAnimCounter += gameTime.ElapsedGameTime.TotalSeconds;
                _spriteBatch.Begin();
                _spriteBatch.Draw(_uiContentStorage.GetHintBackgroundTexture(), Game.GraphicsDevice.Viewport.Bounds,
                    Color.Black);
                var pointCount = ((int)_loadingAnimCounter) % 4;
                _spriteBatch.DrawString(_uiContentStorage.GetButtonFont(), "Loading" + new string('.', pointCount),
                    Game.GraphicsDevice.Viewport.Bounds.Center.ToVector2(), Color.White);
                _spriteBatch.End();
            }
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
                    new PersonMarkersPanel(32 + 8, _uiContentStorage, _sectorViewModel.ViewModelContext, _player,
                        _uiState, _commandPool, _commandFactory);
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

                HandleMainUpdate(_uiState.ActiveActor, gameTime);
            }
            else
            {
                HandleTransition(isInTransition);
            }

            var keyboardState = Keyboard.GetState();
            if (_lastKeyboardState is not null)
            {
                if (_lastKeyboardState.Value.IsKeyDown(Keys.F1) && keyboardState.IsKeyUp(Keys.F1))
                {
                    _showControlTutorial = !_showControlTutorial;
                }
            }

            _lastKeyboardState = keyboardState;
        }

        private void Actor_BeginTransitionToOtherSector(object? sender, EventArgs e)
        {
            _isBeginTransition = true;
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
                _uiState.ActiveActor.Actor.BeginTransitionToOtherSector += Actor_BeginTransitionToOtherSector;
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

        private void BottomMenu_TraitsButtonClicked(object? sender, EventArgs e)
        {
            _personTraitsModal.Show();
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

            if (_personTraitsModal.IsVisible)
            {
                return _personTraitsModal;
            }

            return null;
        }

        private void DrawControlsTutorial(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var controlsTutorialText = UiResources.ControlsTutorialText;
            var spriteFont = _uiContentStorage.GetButtonFont();
            var textSize = spriteFont.MeasureString(controlsTutorialText);
            const int MARGIN = 15;
            spriteBatch.DrawString(
                spriteFont,
                controlsTutorialText,
                new Vector2(graphicsDevice.Viewport.Bounds.Right - textSize.X - MARGIN,
                    graphicsDevice.Viewport.Bounds.Top + MARGIN),
                Color.White);
        }

        private void DrawHud()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _personEffectsPanel.Draw(_spriteBatch);

#if SHOW_NUMS
            DrawStatsNumbers();
#endif

            if (_personMarkerPanel is not null)
            {
                _personMarkerPanel.Draw(_spriteBatch, Game.GraphicsDevice);
            }

            DrawPersonModePanel();

            if (_uiState.HoverViewModel is IActorViewModel actorViewModel)
            {
                DrawMonsterInfo(actorViewModel, _spriteBatch, Game.GraphicsDevice.Viewport.Width,
                    Game.GraphicsDevice.Viewport.Height);
            }

            if (_showControlTutorial)
            {
                DrawControlsTutorial(_spriteBatch, Game.GraphicsDevice);
            }

            var turns = _scoreManager.Scores.Turns;
            var detailedLifetime = ScoreCalculator.ConvertTurnsToDetailed(turns);
            //if (detailedLifetime.Days > 3 && !(_player.MainPerson?.CheckIsDead()).GetValueOrDefault())
            {
                _spriteBatch.DrawString(_uiContentStorage.GetButtonFont(),
                    $"{detailedLifetime.Days}d {detailedLifetime.Hours}h", new Vector2(5, 5), Color.White);
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

            if (_personTraitsModal.IsVisible)
            {
                _personTraitsModal.Draw(_spriteBatch);
            }

            if (_containerModal.IsVisible)
            {
                _containerModal.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }

        private void DrawMonsterInfo(IActorViewModel actorViewModel, SpriteBatch spriteBatch, int viewPortWidth,
            int viewPortHeight)
        {
            if (actorViewModel.Actor.Person is not MonsterPerson monsterPerson)
            {
                return;
            }

            var position = new Vector2(viewPortWidth - 100, viewPortHeight - 100);
            spriteBatch.DrawString(_uiContentStorage.GetAuxTextFont(), MonsterHelper.GetPerkHintText(monsterPerson),
                position, Color.White);
            var isNumbersShow = false;

#if SHOW_NUMS
            isNumbersShow = true;
#endif

            if (isNumbersShow)
            {
                var stats = monsterPerson.GetModule<ISurvivalModule>().Stats;
                var monsterCombatActModule = monsterPerson.GetModule<ICombatActModule>();
                var defaultAct = monsterCombatActModule.GetCurrentCombatActs().First();
                spriteBatch.DrawString(_uiContentStorage.GetAuxTextFont(), GetRollAsString(defaultAct.Efficient),
                    position + new Vector2(0, 16), Color.White);
                for (var statIndex = 0; statIndex < stats.Length; statIndex++)
                {
                    var stat = stats[statIndex];
                    var offsetY = statIndex * 16;
                    var statPosition = new Vector2(0, 32 + offsetY);
                    var statText = $"{stat.Type} - {stat.Value} ({stat.ValueShare:0.##})";
                    spriteBatch.DrawString(_uiContentStorage.GetAuxTextFont(), statText, position + statPosition,
                        Color.White);
                }
            }
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

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members",
            Justification = "Method used in debug with SHOW_NUMS compiler directive.")]
        private void DrawStatsNumbers()
        {
            var mainPerson = _player.MainPerson;
            if (mainPerson is null)
            {
                // Do not display debug info if the player person is not assigned.
                return;
            }

            var stats = mainPerson.GetModule<ISurvivalModule>().Stats;
            var yOffset = STAT_NUMS_X_POSITION;

            for (var statIndex = 0; statIndex < stats.Length; statIndex++)
            {
                var stat = stats[statIndex];
                var statInfo = $"{stat.Type} - {stat.Value} ({stat.ValueShare: 0.##})";
                var itemPosition = statIndex * STAT_ROW_HEIGHT;
                var position = new Vector2(0, yOffset + itemPosition);
                _spriteBatch.DrawString(_uiContentStorage.GetAuxTextFont(), statInfo, position, Color.White);
            }
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

        private static string GetRollAsString(Roll roll)
        {
            return $"{roll.Count}D{roll.Dice} +{roll.Modifiers?.ResultBuff ?? 0}";
        }

        private void HandleMainUpdate(IActorViewModel activeActor, GameTime gameTime)
        {
            var sectorNodeWithPlayerPerson = GetPlayerSectorNode(_player);

            if (sectorNodeWithPlayerPerson != null)
            {
                var sectorWithPlayerPerson = sectorNodeWithPlayerPerson.Sector;
                UpdateCurrentSectorOrPerformTransition(sectorWithPlayerPerson, activeActor, gameTime);
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
                _uiState.ActiveActor.Actor.Person.GetModule<ICombatActModule>().IsCombatMode = false;
                _uiState.ActiveActor.Actor.OpenedContainer -= Actor_OpenedContainer;
                _uiState.ActiveActor.Actor.BeginTransitionToOtherSector -= Actor_BeginTransitionToOtherSector;
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
            IActorViewModel activeActorViewModel,
            GameTime gameTime)
        {
            if (_currentSector == sectorWithPlayerPerson)
            {
                _camera.Follow(activeActorViewModel, Game, gameTime);

                _personEffectsPanel.Update(gameTime);

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