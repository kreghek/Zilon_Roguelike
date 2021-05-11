using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CDT.LIV.MonoGameClient.ViewModels.MainScene;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace CDT.LIV.MonoGameClient.Scenes
{
    class MainScene : GameSceneBase
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly ISectorUiState _uiState;
        private readonly IPlayer _player;
        private readonly ITransitionPool _transitionPool;
        private readonly Camera _camera;

        private SectorViewModel? _sectorViewModel;
        private ISector? _currentSector;

        private bool _isTransitionPerforming;

        public MainScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
            _player = serviceScope.GetRequiredService<IPlayer>();
            _transitionPool = serviceScope.GetRequiredService<ITransitionPool>();

            _camera = new Camera();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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
                    }
                    else if (!_isTransitionPerforming)
                    {
                        _isTransitionPerforming = true;
                        TargetScene = new TransitionScene(Game, _spriteBatch);
                    }
                }
            }
            else
            {
                if (!_isTransitionPerforming)
                {
                    _isTransitionPerforming = true;
                    TargetScene = new TransitionScene(Game, _spriteBatch);
                }
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

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_sectorViewModel != null)
            {
                _sectorViewModel.Draw(gameTime);
            }
        }
    }
}
