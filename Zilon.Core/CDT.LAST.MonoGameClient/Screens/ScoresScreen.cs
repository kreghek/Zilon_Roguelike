namespace CDT.LAST.MonoGameClient.Screens
{
    using System;

    using Engine;
    using Engine;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Resources;
    using Resources;

    using Zilon.Core.Scoring;

    /// <summary>
    /// Scores screen to show a user's score when a character died.
    /// </summary>
    internal class ScoresScreen : GameSceneBase
    {
        private const int RESTART_BUTTON_POSITION_X = 150;

        private const int BUTTON_POSITION_Y = 150;

        private const int BUTTON_WIDTH = 100;

        private const int BUTTON_HEIGHT = 20;

        private const int BUTTON_WIDTH_OFFSET = 100;

        private const int SCORE_MENU_TITLE_POSITION_X = 50;

        private const int SCORE_MENU_TITLE_POSITION_Y = 100;

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
                rect: new Rectangle(
                    x: RESTART_BUTTON_POSITION_X,
                    y: BUTTON_POSITION_Y,
                    width: BUTTON_WIDTH,
                    height: BUTTON_HEIGHT));
            _restartButton.OnClick += RestartButtonClickHandler;

            _goToMainMenu = new TextButton(
                title: UiResources.MainMenuButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(
                    x: RESTART_BUTTON_POSITION_X + BUTTON_WIDTH_OFFSET * 2,
                    y: BUTTON_POSITION_Y,
                    width: BUTTON_WIDTH,
                    height: BUTTON_HEIGHT));
            _goToMainMenu.OnClick += GoToMainMenuButtonClickHandler;

            _goToNextScreen = new TextButton(
                title: UiResources.NextScreenButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(
                    x: RESTART_BUTTON_POSITION_X + BUTTON_WIDTH_OFFSET * 4,
                    y: BUTTON_POSITION_Y,
                    width: BUTTON_WIDTH,
                    height: BUTTON_HEIGHT));
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
                position: new Vector2(x: SCORE_MENU_TITLE_POSITION_X, y: SCORE_MENU_TITLE_POSITION_Y),
                color: Color.White);
            _spriteBatch.DrawString(
                spriteFont: font,
                text: _scoreSummary,
                position: new Vector2(x: SCORE_MENU_TITLE_POSITION_X * 3, y: SCORE_MENU_TITLE_POSITION_Y * 2),
                color: Color.White);

            _spriteBatch.End();
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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