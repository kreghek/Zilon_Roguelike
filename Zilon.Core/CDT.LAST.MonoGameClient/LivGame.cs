﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.GameComponents;
using CDT.LAST.MonoGameClient.Screens;
using CDT.LAST.MonoGameClient.ViewModels.MainScene;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient
{
    public class LivGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly ServiceProvider _serviceProvider;
        private Texture2D? _cursorTexture;
        private SpriteBatch? _spriteBatch;

        public LivGame(ServiceProvider serviceProvider)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _serviceProvider = serviceProvider;
        }

        public GraphicsDeviceManager Graphics => _graphics;

        public ServiceProvider ServiceProvider => _serviceProvider;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_spriteBatch is null)
            {
                throw new InvalidOperationException();
            }

            base.Draw(gameTime);

            // Print mouse position and draw cursor itself

            var mouseState = Mouse.GetState();
            _spriteBatch.Begin();
            _spriteBatch.Draw(_cursorTexture, new Vector2(mouseState.X, mouseState.Y), Color.White);
            _spriteBatch.End();
        }

        protected override void Initialize()
        {
            InitGlobeLoop();

            InitCommandLoop();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _cursorTexture = Content.Load<Texture2D>("Sprites/ui/walk-cursor");

            var uiContentStorage = ServiceProvider.GetRequiredService<IUiContentStorage>();
            uiContentStorage.LoadContent(Content);

            var personVisualizationContentStorage =
                ServiceProvider.GetRequiredService<IPersonVisualizationContentStorage>();
            personVisualizationContentStorage.LoadContent(Content);

            var gameobjectVisualizationContentStorage =
                ServiceProvider.GetRequiredService<IGameObjectVisualizationContentStorage>();
            gameobjectVisualizationContentStorage.LoadContent(Content);

            var personSoundContentStorage = ServiceProvider.GetRequiredService<IPersonSoundContentStorage>();
            personSoundContentStorage.LoadContent(Content);

            var uiSoundStorage = ServiceProvider.GetRequiredService<IUiSoundStorage>();
            uiSoundStorage.LoadContent(Content);
            UiThemeManager.SoundStorage = uiSoundStorage;

            var sceneManager = new ScreenManager(this);
            var titleScene = new TitleScreen(this, _spriteBatch);
            sceneManager.ActiveScreen = titleScene;

            Components.Add(sceneManager);

            var font = Content.Load<SpriteFont>("Fonts/Main");
#if DEBUG
            var fpsCounter = new FpsCounter(this, _spriteBatch, font);
            Components.Add(fpsCounter);

            var cheatInput = new CheatInput(this, _spriteBatch, font);
            Components.Add(cheatInput);

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.ApplyChanges();
#endif

            var soundtrackManagerComponent = new SoundtrackManagerComponent(this);
            var soundtrackManager = ServiceProvider.GetRequiredService<SoundtrackManager>();
            var titleSong = Content.Load<Song>("Audio/TitleBackgroundTrack");
            soundtrackManager.Initialize(titleSong);
            soundtrackManagerComponent.Initialize(soundtrackManager);
            Components.Add(soundtrackManagerComponent);
#if !DEBUG
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth
 = 1280;
            _graphics.PreferredBackBufferHeight
 = 720;
            _graphics.ApplyChanges();
#endif

            var versionDisplay = new VersionDisplay(this, _spriteBatch, font);
            Components.Add(versionDisplay);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void InitCommandLoop()
        {
            var commandLoop = _serviceProvider.GetRequiredService<ICommandLoopUpdater>();

            var playerState = _serviceProvider.GetRequiredService<ISectorUiState>();
            var inventoryState = _serviceProvider.GetRequiredService<IInventoryState>();
            commandLoop.CommandProcessed += (s, e) =>
            {
                inventoryState.SelectedProp = null;
                playerState.SelectedViewModel = null;
                playerState.TacticalAct = null;
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