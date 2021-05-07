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
        private SectorViewModel? _sectorViewModel;
        private Camera _camera;

        private ISector? _currentSector; 

        public MainScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
            _player = serviceScope.GetRequiredService<IPlayer>();

            _camera = new Camera();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_sectorViewModel is null)
            {
                _sectorViewModel = new SectorViewModel(Game, _camera, _spriteBatch);

                _currentSector = _sectorViewModel.Sector;

                StartBackgroundLoops();
            }

            if (_uiState.ActiveActor != null)
            {
                var sectorNode = GetPlayerSectorNode(_player);

                if (sectorNode != null)
                {
                    _camera.Follow(_uiState.ActiveActor, Game);
                }

                if (sectorNode != null && sectorNode.Sector != _currentSector)
                { 

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

        private void StartBackgroundLoops()
        {
            var serviceScope = ((LivGame)Game).ServiceProvider;

            var globeLoopUpdater = serviceScope.GetRequiredService<IGlobeLoopUpdater>();

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            globeLoopUpdater.ErrorOccured += (s, e) => 
            {
                Debug.WriteLine(e.Exception.ToString()); 
            };

            var commandLoop = serviceScope.GetRequiredService<ICommandLoopUpdater>();

            commandLoop.ErrorOccured += (s, e) => 
            { 
                Debug.WriteLine(e.Exception.ToString());
            };
            commandLoop.CommandAutoExecuted += (s, e) => { Debug.WriteLine("Auto execute last command"); };
            var playerState = serviceScope.GetRequiredService<ISectorUiState>();
            var inventoryState = serviceScope.GetRequiredService<IInventoryState>();
            commandLoop.CommandProcessed += (s, e) =>
            {
                inventoryState.SelectedProp = null;
                playerState.SelectedViewModel = null;
            };
            var commandLoopTask = commandLoop.StartAsync(cancellationToken);
            commandLoopTask.ContinueWith(task => Debug.WriteLine(task.Exception),
                TaskContinuationOptions.OnlyOnFaulted);
            commandLoopTask.ContinueWith(task => Debug.WriteLine("Game loop stopped."),
                TaskContinuationOptions.OnlyOnCanceled);
        }
    }
}
