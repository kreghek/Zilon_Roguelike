using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal class PersonMarkersPanel
    {
        private const int MARKER_WIDTH = 16;
        private const int MARKER_HEGHT = 32;

        private readonly ServiceProviderCommandFactory _commandFactory;
        private readonly ICommandPool _commandPool;
        private readonly IList<Marker> _drawnItemList;
        private readonly IPlayer _player;
        private readonly int _positionOffsetY;
        private readonly ISectorUiState _sectorUiState;
        private readonly SectorViewModelContext _sectorViewModelContext;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly IList<ActorViewModel> _visibleActors;

        public PersonMarkersPanel(int positionOffsetY,
            IUiContentStorage uiContentStorage,
            SectorViewModelContext sectorViewModelContext,
            IPlayer player,
            ISectorUiState sectorUiState,
            ICommandPool commandPool,
            ServiceProviderCommandFactory commandFactory)
        {
            _positionOffsetY = positionOffsetY;
            _uiContentStorage = uiContentStorage;
            _sectorViewModelContext = sectorViewModelContext;
            _player = player;
            _sectorUiState = sectorUiState;
            _commandPool = commandPool;
            _commandFactory = commandFactory;
            _visibleActors = new List<ActorViewModel>();
            _drawnItemList = new List<Marker>();
        }

        public static bool MouseIsOver { get; set; }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var index = 0;

            var viewPortHalfWidth = graphicsDevice.Viewport.Width / 2;
            var viewPortHeight = graphicsDevice.Viewport.Height;

            _drawnItemList.Clear();
            if (!_sectorUiState.CanPlayerGivesCommand)
            {
                return;
            }

            var orderedVisibleActors = _visibleActors.OrderBy(x =>
            {
                var map = _sectorViewModelContext.Sector.Map;
                var activeActorNode = _sectorUiState.ActiveActor.Actor.Node;
                var monsterNode = x.Node;
                return map.DistanceBetween(activeActorNode, monsterNode);
            }).ThenBy(x => x.Actor.Person.Id).ToArray();

            foreach (var item in orderedVisibleActors)
            {
                var itemOffsetX = index * MARKER_WIDTH;
                var rect = new Rectangle(
                    viewPortHalfWidth + itemOffsetX,
                    viewPortHeight - _positionOffsetY - MARKER_HEGHT,
                    MARKER_WIDTH,
                    MARKER_HEGHT
                );

                var testedCombatAct = _sectorUiState.TacticalAct;
                if (testedCombatAct is null && _sectorUiState.ActiveActor != null)
                {
                    testedCombatAct = GetDefaultCombatAct(_sectorUiState);
                }

                if (testedCombatAct is not null)
                {

                    var selectedCombatActRange = testedCombatAct.Stats.Range;
                    var map = _sectorViewModelContext.Sector.Map;
                    var activeActorNode = _sectorUiState.ActiveActor.Actor.Node;
                    var monsterNode = item.Actor.Node;
                    if (selectedCombatActRange.Contains(map.DistanceBetween(activeActorNode, monsterNode)))
                    {
                        spriteBatch.Draw(_uiContentStorage.GetPersonMarkerTextureSheet(), rect, new Rectangle(0,0,16,32), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(_uiContentStorage.GetPersonMarkerTextureSheet(), rect, new Rectangle(16, 0, 16, 32), Color.White);
                    }

                    _drawnItemList.Add(new Marker(rect, item));
                }
                else
                {
                    // Old behaviour.
                    spriteBatch.Draw(_uiContentStorage.GetPersonMarkerTextureSheet(), rect, Color.White);
                    _drawnItemList.Add(new Marker(rect, item));
                }

                index++;
            }
        }

        public void Update()
        {
            _visibleActors.Clear();

            var actorViewModels = _sectorViewModelContext.GameObjects.OfType<ActorViewModel>().ToArray();
            foreach (var actorViewModel in actorViewModels)
            {
                if (actorViewModel.Actor.Person != _player.MainPerson
                    && actorViewModel.CanDraw)
                {
                    _visibleActors.Add(actorViewModel);
                }
            }

            CheckActorUnderHover();
        }

        private void CheckActorUnderHover()
        {
            var mouse = Mouse.GetState();
            var mouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);

            foreach (var item in _drawnItemList)
            {
                item.ActorViewModel.IsGraphicsOutlined = item.Rect.Intersects(mouseRect);

                if (item.ActorViewModel.IsGraphicsOutlined)
                {
                    _sectorUiState.HoverViewModel = item.ActorViewModel;

                    HandleMarkerClick(mouse, item);
                }
            }

            MouseIsOver = _drawnItemList.Any(x => x.ActorViewModel.IsGraphicsOutlined);
        }

        private void HandleMarkerClick(MouseState mouse, Marker item)
        {
            if (!_sectorUiState.CanPlayerGivesCommand)
            {
                return;
            }

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (_sectorUiState.TacticalAct is null)
                {
                    SelectPunchAsDefaultCombatAct(_sectorUiState);
                }

                var attackCommand = _commandFactory.GetCommand<AttackCommand>();
                _sectorUiState.HoverViewModel = item.ActorViewModel;
                if (attackCommand.CanExecute().IsSuccess)
                {
                    _sectorUiState.SelectedViewModel = item.ActorViewModel;
                    _commandPool.Push(attackCommand);
                }
            }
        }

        private static void SelectPunchAsDefaultCombatAct(ISectorUiState uiState)
        {
            uiState.TacticalAct = GetDefaultCombatAct(uiState);
        }

        private static ICombatAct GetDefaultCombatAct(ISectorUiState uiState)
        {
            var availableCombatActs =
                uiState.ActiveActor.Actor.Person.GetModule<ICombatActModule>().GetCurrentCombatActs();
            var punchAct = availableCombatActs.Single(x => x.Scheme.Sid == "punch");

            return punchAct;
        }

        private record Marker(Rectangle Rect, ActorViewModel ActorViewModel);
    }
}