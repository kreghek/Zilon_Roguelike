using System;
using System.Globalization;
using System.Threading;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Scenes
{
    internal class TitleScene : GameSceneBase
    {
        private readonly GlobeGenerationScene _globeGenerationScene;
        private readonly SpriteBatch _spriteBatch;
        private readonly Button _startButton;
        private readonly Button _switchLanguageButton;

        public TitleScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            _globeGenerationScene = new GlobeGenerationScene(game, spriteBatch);

            var buttonTexture = game.Content.Load<Texture2D>("Sprites/ui/button");
            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _startButton = new Button(UiResources.StartGameButtonTitle, buttonTexture, font,
                new Rectangle(150, 150, 100, 20));
            _startButton.OnClick += StartButtonClickHandler;

            _switchLanguageButton = new Button("Switch lang", buttonTexture, font, new Rectangle(150, 200, 100, 20));

            _switchLanguageButton.OnClick += SwitchLanguageButtonClickHandler;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            _startButton.Draw(_spriteBatch);
            _switchLanguageButton.Draw(_spriteBatch);

            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _spriteBatch.DrawString(font, "Title", new Vector2(50, 100), Color.White);
            _spriteBatch.DrawString(font, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName,
                new Vector2(150, 100), Color.White);

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

            _startButton.Update();
            _switchLanguageButton.Update();
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
                var newCulture = new CultureInfo("ru-RU");
                Thread.CurrentThread.CurrentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
            }
            else
            {
                var newCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
            }
        }
    }
}