using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client.Sector;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class GlobeSelectionScreen : GameSceneBase
    {
        private const string START_LOCATION_SID = "intro";
        private readonly ICommandLoopUpdater _commandLoop;

        private readonly TextButton _generateButton;
        private readonly IGlobeInitializer _globeInitializer;
        private readonly IGlobeLoopUpdater _globeLoop;
        private readonly SpriteBatch _spriteBatch;
        private readonly IUiContentStorage _uiContentStorage;

        private bool _generationWasStarted;

        public GlobeSelectionScreen(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceProvider = ((LivGame)game).ServiceProvider;

            _uiContentStorage = serviceProvider.GetRequiredService<IUiContentStorage>();
            _globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();
            _globeLoop = serviceProvider.GetRequiredService<IGlobeLoopUpdater>();
            _commandLoop = serviceProvider.GetRequiredService<ICommandLoopUpdater>();

            var buttonTexture = _uiContentStorage.GetButtonTexture();
            var font = _uiContentStorage.GetButtonFont();

            _generateButton = new TextButton(UiResources.GenerateGlobeButtonTitle, buttonTexture, font,
                new Rectangle(150, 150, 100, 20));

            _generateButton.OnClick += GenerateButtonClickHandlerAsync;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            var font = _uiContentStorage.GetButtonFont();

            _spriteBatch.DrawString(font, "Генерация мира", new Vector2(100, 100), Color.White);

            _generateButton.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _generateButton.Update();
        }

        private void ClearPreviousState()
        {
            if (_globeLoop.IsStarted && _commandLoop.IsStarted)
            {
                // Means the game restarted.
                // Ways to restart:
                // - From the score screen.
                // - From the leader screen.

                _globeLoop.Stop();
                _commandLoop.StopAsync().Wait(10_000);
            }
            else if (!_globeLoop.IsStarted && !_commandLoop.IsStarted)
            {
                // Means game started first time.
                // Do nothing. The game state is clean yet.
            }
            else
            {
                Debug.Fail("Unknown state.");

                // There are no cases to have one of loop been started and other been stoped.
                // But try to clear loops anyway.

                _globeLoop.Stop();
                _commandLoop.StopAsync().Wait(10_000);
            }
        }

        private async void GenerateButtonClickHandlerAsync(object? sender, EventArgs e)
        {
            if (_generationWasStarted)
            {
                // Ignore next clicks to avoid multiple globe creations.
                return;
            }

            _generationWasStarted = true;

            var generateGlobeTask = Task.Run(async () =>
            {
                ClearPreviousState();

                var globe = await _globeInitializer.CreateGlobeAsync(START_LOCATION_SID).ConfigureAwait(false);

                if (globe is null)
                {
                    throw new InvalidOperationException();
                }

                _globeLoop.Start();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _commandLoop.StartAsync(CancellationToken.None);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            });

            await generateGlobeTask;

            TargetScene = new MainScreen(Game, _spriteBatch);
        }
    }
}