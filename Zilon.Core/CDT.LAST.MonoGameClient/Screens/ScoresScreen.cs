namespace CDT.LAST.MonoGameClient.Screens
{
    using System;

    using CDT.LAST.MonoGameClient.Engine;
    using CDT.LAST.MonoGameClient.Resources;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using Zilon.Core.Scoring;

    /// <summary>
    /// Scores screen to show a user's score when a character died.
    /// </summary>
    internal class ScoresScreen : GameSceneBase
    {
        private readonly GlobeSelectionScreen _globeGenerationScene;

        private readonly TextButton _goToMainMenu;

        private readonly TextButton _goToNextScreen;

        private readonly TextButton _restartButton;

        private readonly string _scoreSummary;

        private readonly SpriteBatch _spriteBatch;

        private readonly IUiContentStorage _uiContentStorage;

        /// <inheritdoc />
        public ScoresScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            _spriteBatch = spriteBatch;
            
            var serviceScope = ((LivGame)Game).ServiceProvider;
            var scoreManager = serviceScope.GetRequiredService<IScoreManager>();
            _scoreSummary = TextSummaryHelper.CreateTextSummary(scoreManager.Scores);
            _uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();

            _globeGenerationScene = new GlobeSelectionScreen(game: game, spriteBatch: spriteBatch);

            var buttonTexture = _uiContentStorage.GetButtonTexture();
            var font = _uiContentStorage.GetButtonFont();

            _restartButton = new TextButton(
                title: UiResources.StartGameButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(x: 150, y: 150, width: 100, height: 20));
            _restartButton.OnClick += RestartButtonClickHandler;

            _goToMainMenu = new TextButton(
                title: UiResources.MainMenuButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(x: 350, y: 150, width: 100, height: 20));
            _goToMainMenu.OnClick += GoToMainMenuButtonClickHandler;

            _goToNextScreen = new TextButton(
                title: UiResources.NextScreenButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(x: 550, y: 150, width: 100, height: 20));
            _goToNextScreen.OnClick += GoToNextScreenButtonClickHandler;
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            _restartButton.Draw(_spriteBatch);
            _goToMainMenu.Draw(_spriteBatch);
            _goToNextScreen.Draw(_spriteBatch);

            var font = _uiContentStorage.GetButtonFont();

            _spriteBatch.DrawString(
                spriteFont: font,
                text: UiResources.ScoreMenuTitle,
                position: new Vector2(x: 50, y: 100),
                color: Color.White);
            _spriteBatch.DrawString(
                spriteFont: font,
                text: _scoreSummary,
                position: new Vector2(x: 150, y: 200),
                color: Color.White);

            _spriteBatch.End();
        }

        /// <inheritdoc />
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
            TargetScene = new TitleScreen(game: Game, spriteBatch: _spriteBatch);
        }

        private void GoToNextScreenButtonClickHandler(object? sender, EventArgs e)
        {
            TargetScene = new LeaderBoardScreen(game: Game, spriteBatch: _spriteBatch);
        }

        private void RestartButtonClickHandler(object? sender, EventArgs e)
        {
            TargetScene = _globeGenerationScene;
        }
    }
}