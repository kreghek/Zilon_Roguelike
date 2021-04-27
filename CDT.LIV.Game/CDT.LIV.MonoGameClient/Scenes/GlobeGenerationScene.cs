using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.World;

namespace CDT.LIV.MonoGameClient.Scenes
{
    class GlobeGenerationScene : GameSceneBase
    {
        private bool _generationWasStarted;
        private readonly SpriteBatch _spriteBatch;
        private readonly MainScene _mainScene;

        public GlobeGenerationScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
            _mainScene = new MainScene(game, spriteBatch);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _spriteBatch.DrawString(font, "Генерация мира", new Vector2(100, 100), Color.White);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Poll for current keyboard state
            var state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
                Game.Exit();

            if (state.IsKeyDown(Keys.Up))
            {
                if (!_generationWasStarted)
                {
                    _generationWasStarted = true;

                    var generateGlobeTask = Task.Run(async () =>
                    {
                        var serviceScope = ((LivGame)Game).ServiceProvider;
                        var globeInitializer = serviceScope.GetRequiredService<IGlobeInitializer>();
                        var globe = await globeInitializer.CreateGlobeAsync("intro").ConfigureAwait(false);

                        if (globe is null)
                        {
                            throw new InvalidOperationException();
                        }

                        var player = serviceScope.GetRequiredService<IPlayer>();

                        if (player is null)
                        {
                            throw new InvalidOperationException();
                        }

                        var gameLoop = new GameLoop(globe, player);

                        var processTask = gameLoop.StartProcessAsync(CancellationToken.None);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        processTask.ContinueWith(task => Console.WriteLine(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
                        processTask.ContinueWith(task => Console.WriteLine("Game loop stopped."),
                            TaskContinuationOptions.OnlyOnCanceled);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


                    });

                    generateGlobeTask.ContinueWith((task) =>
                    {
                        TargetScene = _mainScene;
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                    generateGlobeTask.ContinueWith(task => 
                    { 

                    }, TaskContinuationOptions.OnlyOnFaulted);
                }
            }
        }
    }

    internal class GameLoop
    {
        private readonly IGlobe _globe;
        private readonly IPlayer _player;

        public GameLoop(IGlobe globe, IPlayer player)
        {
            _globe = globe ?? throw new ArgumentNullException(nameof(globe));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public async Task StartProcessAsync(CancellationToken cancellationToken)
        {
            if (_player.MainPerson is null)
            {
                return;
            }

            var playerPersonSurvivalModule = _player.MainPerson.GetModule<ISurvivalModule>();
            while (!playerPersonSurvivalModule.IsDead)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _globe.UpdateAsync(CancellationToken.None).ConfigureAwait(false);
            }
        }
    }
}
