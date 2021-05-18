using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client.Sector;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.Scenes
{
    internal class GlobeGenerationScene : GameSceneBase
    {
        public static string? _lastError = "";
        private readonly MainScene _mainScene;
        private readonly Button _generateButton;
        private readonly SpriteBatch _spriteBatch;
        private bool _generationWasStarted;


        public GlobeGenerationScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
            _mainScene = new MainScene(game, spriteBatch);

            var buttonTexture = game.Content.Load<Texture2D>("Sprites/ui/button");
            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _generateButton = new Button("generate", buttonTexture, font, new Rectangle(150, 150, 100, 20))
            {
                Click = GenerateButtonClickHandler
            };
        }

        private void GenerateButtonClickHandler()
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

                    var gameLoop = serviceScope.GetRequiredService<IGlobeLoopUpdater>();

                    gameLoop.Start();

                    var commandLoop = serviceScope.GetRequiredService<ICommandLoopUpdater>();
                    var commandLoopTask = commandLoop.StartAsync(CancellationToken.None);
                    commandLoopTask.ContinueWith(task => _lastError += task.Exception.ToString(),
                        TaskContinuationOptions.OnlyOnFaulted);
                    commandLoopTask.ContinueWith(task => _lastError += "Game loop stopped.",
                        TaskContinuationOptions.OnlyOnCanceled);
                });

                generateGlobeTask.ContinueWith(task =>
                {
                    TargetScene = _mainScene;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                generateGlobeTask.ContinueWith(task =>
                {
                    Debug.WriteLine(task.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _spriteBatch.DrawString(font, "Генерация мира", new Vector2(100, 100), Color.White);
            _spriteBatch.DrawString(font, _lastError, new Vector2(100, 120), Color.White);
            _generateButton.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Poll for current keyboard state
            var state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
            {
                Game.Exit();
            }

            _generateButton.Update();
        }
    }
}