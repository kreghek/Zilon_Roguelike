using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using CDT.LIV.MonoGameClient.Scenes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;

namespace CDT.LIV.MonoGameClient
{
    public class LivGame : Game
    {
        private readonly ServiceProvider _serviceProvider;
        private GraphicsDeviceManager? _graphics;
        private SpriteBatch? _spriteBatch;

        public LivGame(ServiceProvider serviceProvider)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _serviceProvider = serviceProvider;
        }

        public ServiceProvider ServiceProvider => _serviceProvider;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            InitGlobeLoop();

            InitCommandLoop();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var sceneManager = new SceneManager(this);
            var titleScene = new TitleScene(this, _spriteBatch);
            sceneManager.ActiveScene = titleScene;

            Components.Add(sceneManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void InitCommandLoop()
        {
            var commandLoop = _serviceProvider.GetRequiredService<ICommandLoopUpdater>();

            commandLoop.ErrorOccured += (s, e) =>
            {
                GlobeGenerationScene._lastError += e.Exception.ToString();
            };
            commandLoop.CommandAutoExecuted += (s, e) =>
            {
                GlobeGenerationScene._lastError += "Auto execute last command";
            };
            var playerState = _serviceProvider.GetRequiredService<ISectorUiState>();
            var inventoryState = _serviceProvider.GetRequiredService<IInventoryState>();
            commandLoop.CommandProcessed += (s, e) =>
            {
                inventoryState.SelectedProp = null;
                playerState.SelectedViewModel = null;
            };
        }

        private void InitGlobeLoop()
        {
            var globeLoop = _serviceProvider.GetRequiredService<IGlobeLoopUpdater>();

            globeLoop.ErrorOccured += (s, e) =>
            {
                Debug.WriteLine(e.Exception.ToString());
            };
        }
    }
}