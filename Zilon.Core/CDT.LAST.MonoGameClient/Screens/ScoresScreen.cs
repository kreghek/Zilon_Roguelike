namespace CDT.LAST.MonoGameClient.Screens
{
    using System;

    using CDT.LAST.MonoGameClient.Engine;
    using CDT.LAST.MonoGameClient.Resources;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using Zilon.Core.Scoring;

    internal class ScoresScreen : GameSceneBase
    {
        private readonly GlobeSelectionScreen _globeGenerationScene;

        private readonly TextButton _goToMainMenu;

        private readonly TextButton _goToNextScreen;

        private readonly TextButton _restartButton;

        private readonly string _scoreSummary;

        private readonly SpriteBatch _spriteBatch;

        public ScoresScreen(Game game, SpriteBatch spriteBatch, IScoreManager scoreManager)
            : base(game)
        {
            _spriteBatch = spriteBatch;

            _globeGenerationScene = new GlobeSelectionScreen(game: game, spriteBatch: spriteBatch);

            var buttonTexture = game.Content.Load<Texture2D>("Sprites/ui/button");
            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _restartButton = new TextButton(
                title: UiResources.StartGameButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(x: 150, y: 150, width: 100, height: 20));
            _restartButton.OnClick += RestartButtonClickHandler;

            _goToMainMenu = new TextButton(
                title: UiResources.StartGameButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(x: 150, y: 150, width: 100, height: 20));
            _goToMainMenu.OnClick += GoToMainMenuButtonClickHandler;

            _goToNextScreen = new TextButton(
                title: UiResources.StartGameButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(x: 150, y: 150, width: 100, height: 20));
            _goToNextScreen.OnClick += GoToNextScreenButtonClickHandler;

            _scoreSummary = TextSummaryHelper.CreateTextSummary(scoreManager.Scores);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            _restartButton.Draw(_spriteBatch);
            _goToMainMenu.Draw(_spriteBatch);

            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _spriteBatch.DrawString(
                spriteFont: font,
                text: "Score menu",
                position: new Vector2(x: 50, y: 100),
                color: Color.White);
            _spriteBatch.DrawString(
                spriteFont: font,
                text: _scoreSummary,
                position: new Vector2(x: 150, y: 200),
                color: Color.White);

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

            _restartButton.Update();
            _goToMainMenu.Update();
            _goToNextScreen.Update();
        }

        private void GoToMainMenuButtonClickHandler(object? sender, EventArgs e)
        {
            //TODO:
        }

        private void GoToNextScreenButtonClickHandler(object? sender, EventArgs e)
        {
            //TODO:
        }

        private void RestartButtonClickHandler(object? sender, EventArgs e)
        {
            //TODO:
        }
    }
}