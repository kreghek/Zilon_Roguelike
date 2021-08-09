using System;
using System.Globalization;
using System.Threading;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.GameComponents;
using CDT.LAST.MonoGameClient.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class TitleScreen : GameSceneBase
    {
        private readonly GlobeSelectionScreen _globeGenerationScene;
        private readonly SpriteBatch _spriteBatch;
        private readonly TextButton _startButton;
        private readonly TextButton _switchLanguageButton;
        private readonly TextButton _switchResolutionButton;

        public TitleScreen(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            _globeGenerationScene = new GlobeSelectionScreen(game, spriteBatch);

            var buttonTexture = game.Content.Load<Texture2D>("Sprites/ui/button");
            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _startButton = new TextButton(UiResources.StartGameButtonTitle, buttonTexture, font,
                new Rectangle(Game.GraphicsDevice.Viewport.Bounds.Center.X, 150, 100, 20));
            _startButton.OnClick += StartButtonClickHandler;

            _switchLanguageButton = new TextButton(UiResources.SwitchLanguagebuttonTitle,
                buttonTexture,
                font,
                new Rectangle(Game.GraphicsDevice.Viewport.Bounds.Center.X, 200, 100, 20));

            _switchLanguageButton.OnClick += SwitchLanguageButtonClickHandler;

            _switchResolutionButton = new TextButton(UiResources.SwitchResolutionButtonTitle,
                buttonTexture,
                font,
                new Rectangle(Game.GraphicsDevice.Viewport.Bounds.Center.X, 250, 100, 20));
            _switchResolutionButton.OnClick += SwitchResolutionButton_OnClick;
        }

        private void SwitchResolutionButton_OnClick(object? sender, EventArgs e)
        {
            var graphicsManager = ((LivGame)Game).Graphics;
            if (graphicsManager.PreferredBackBufferWidth == 800)
            {
                graphicsManager.IsFullScreen = true;
                graphicsManager.PreferredBackBufferWidth = 1920;
                graphicsManager.PreferredBackBufferHeight = 1080;
                graphicsManager.ApplyChanges();
            }
            else if (graphicsManager.PreferredBackBufferWidth == 1920)
            {
                graphicsManager.IsFullScreen = true;
                graphicsManager.PreferredBackBufferWidth = 1280;
                graphicsManager.PreferredBackBufferHeight = 720;
                graphicsManager.ApplyChanges();
            }
            else if (graphicsManager.PreferredBackBufferWidth == 1280)
            {
                graphicsManager.IsFullScreen = false;
                graphicsManager.PreferredBackBufferWidth = 800;
                graphicsManager.PreferredBackBufferHeight = 480;
                graphicsManager.ApplyChanges();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            var font = Game.Content.Load<SpriteFont>("Fonts/Main");
            var titleFont = Game.Content.Load<SpriteFont>("Fonts/Scores");

            const int BUTTON_WIDTH = 100;
            const int BUTTON_HEIGHT = 20;

            var logoSize = titleFont.MeasureString("LAST IMPERIAL VAGABOND");

            _spriteBatch.DrawString(titleFont, "LAST IMPERIAL VAGABOND", new Vector2(Game.GraphicsDevice.Viewport.Bounds.Center.X - logoSize.X / 2, 100), Color.White);

            _startButton.Title = UiResources.StartGameButtonTitle;
            _startButton.Rect = new Rectangle(Game.GraphicsDevice.Viewport.Bounds.Center.X - BUTTON_WIDTH / 2, 150, BUTTON_WIDTH, BUTTON_HEIGHT);
            _startButton.Draw(_spriteBatch);

            _switchLanguageButton.Title = UiResources.SwitchLanguagebuttonTitle;
            _switchLanguageButton.Rect = new Rectangle(Game.GraphicsDevice.Viewport.Bounds.Center.X - BUTTON_WIDTH / 2, 200, BUTTON_WIDTH, BUTTON_HEIGHT);
            _switchLanguageButton.Draw(_spriteBatch);

            _spriteBatch.DrawString(font, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName,
                new Vector2(_switchLanguageButton.Rect.Right + 5, _switchLanguageButton.Rect.Top), Color.White);

            _switchResolutionButton.Title = UiResources.SwitchResolutionButtonTitle;
            _switchResolutionButton.Rect = new Rectangle(Game.GraphicsDevice.Viewport.Bounds.Center.X - BUTTON_WIDTH / 2, 250, BUTTON_WIDTH, BUTTON_HEIGHT);
            _switchResolutionButton.Draw(_spriteBatch);

            var graphicsManager = ((LivGame)Game).Graphics;
            _spriteBatch.DrawString(font, $"{graphicsManager.PreferredBackBufferWidth} x {graphicsManager.PreferredBackBufferHeight}",
                new Vector2(_switchResolutionButton.Rect.Right + 5, _switchResolutionButton.Rect.Top), Color.White);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ((LivGame)Game).ServiceProvider.GetRequiredService<SoundtrackManager>().PlayBackgroundTrack();

            // Poll for current keyboard state
            var state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
            {
                Game.Exit();
            }

            _startButton.Update();
            _switchLanguageButton.Update();
            _switchResolutionButton.Update();
        }

        private void StartButtonClickHandler(object? sender, EventArgs e)
        {
            TargetScene = _globeGenerationScene;
        }

        private void SwitchLanguageButtonClickHandler(object? sender, EventArgs e)
        {
            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            if (string.Equals(currentLanguage.TwoLetterISOLanguageName, "en",
                StringComparison.InvariantCultureIgnoreCase))
            {
                var newCulture = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
                Thread.CurrentThread.CurrentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
            }
            else
            {
                var newCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
            }
        }
    }
}