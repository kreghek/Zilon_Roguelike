using System;

using CDT.LAST.MonoGameClient.Database;
using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Scoring;

namespace CDT.LAST.MonoGameClient.Screens
{
    /// <summary>
    /// Scores screen to show a user's score when a character died.
    /// </summary>
    internal class ScoresScreen : GameSceneBase
    {
        private const int RESTART_BUTTON_POSITION_X = 150;

        private const int BUTTON_POSITION_Y = 150;

        private const int BUTTON_WIDTH = 100;

        private const int BUTTON_HEIGHT = 20;

        private const int SCORE_MENU_TITLE_POSITION_X = 50;

        private const int SCORE_MENU_TITLE_POSITION_Y = 100;

        private readonly TextButton _restartButton;
        private readonly IScoreManager _scoreManager;
        private readonly string _scoreSummary;

        private readonly SpriteBatch _spriteBatch;

        private readonly IUiContentStorage _uiContentStorage;

        private readonly DbContext _dbContext;

        /// <inheritdoc />
        public ScoresScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _scoreManager = serviceScope.GetRequiredService<IScoreManager>();
            _scoreSummary = TextSummaryHelper.CreateTextSummary(_scoreManager.Scores);
            _uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();

            _dbContext = serviceScope.GetRequiredService<DbContext>();

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
            _restartButton.OnClick += GoToNextScreenButtonClickHandler;
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var font = _uiContentStorage.GetButtonFont();

            _spriteBatch.Begin();

            _spriteBatch.DrawString(
                spriteFont: font,
                text: _scoreSummary,
                position: new Vector2(x: SCORE_MENU_TITLE_POSITION_X * 3, y: SCORE_MENU_TITLE_POSITION_Y * 2),
                color: Color.White);

            _restartButton.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _restartButton.Update();
        }

        private void GoToNextScreenButtonClickHandler(object? sender, EventArgs e)
        {
            _dbContext.AppendScores("Безымянный бродяга", _scoreSummary);

            _scoreManager.ResetScores();

            TargetScene = new LeaderBoardScreen(game: Game, spriteBatch: _spriteBatch);
        }
    }
}