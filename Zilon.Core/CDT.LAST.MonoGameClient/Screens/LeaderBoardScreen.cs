namespace CDT.LAST.MonoGameClient.Screens
{
    using System;
    using System.Collections.Generic;

    using CDT.LAST.MonoGameClient.Database;
    using CDT.LAST.MonoGameClient.Engine;
    using CDT.LAST.MonoGameClient.Resources;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The leaderboard screen displays all the results of all played users.
    /// </summary>
    public class LeaderBoardScreen : GameSceneBase
    {
        private const int GO_TO_MAIN_MENU_BUTTON_POSITION_X = LEADERBOARD_MENU_TITLE_POSITION_X;

        private const int GO_TO_MAIN_MENU_BUTTON_POSITION_Y = 150;

        private const int BUTTON_WIDTH = 100;

        private const int BUTTON_HEIGHT = 20;

        private const int LEADERBOARD_MENU_TITLE_POSITION_X = 50;

        private const int LEADERBOARD_MENU_TITLE_POSITION_Y = 100;

        private const int PLAYER_NICKNAME_TITLE_POSITION_X = PLAYER_NUMBER_TITLE_POSITION_X * 3;

        private const int PLAYER_NUMBER_TITLE_POSITION_X = LEADERBOARD_MENU_TITLE_POSITION_X;

        private const int PLAYER_SCORE_TITLE_POSITION_X = PLAYER_NICKNAME_TITLE_POSITION_X * 6;

        private const int ROW_STEP = 50;

        private const int FIRST_TABLE_ROW_POSITION_Y = GO_TO_MAIN_MENU_BUTTON_POSITION_Y + ROW_STEP;

        private const int SCORE_TABLE_HEADERS_ROW_OFFSET_Y = 2;

        private const int TABLE_RESULTS_OFFSET_Y = 3;

        private readonly DbContext _dbContext;

        private readonly SpriteFont _font;

        private readonly TextButton _goToMainMenu;

        private readonly SpriteBatch _spriteBatch;

        private readonly Color _tableHeaderColor = Color.White;

        private readonly Color _tableResultsColor = Color.White;

        private readonly List<PlayerScore> _leaderBoardRecords;

        /// <inheritdoc />
        public LeaderBoardScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            var uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();

            _dbContext = serviceScope.GetRequiredService<DbContext>();

            _goToMainMenu = new TextButton(
                UiResources.MainMenuButtonTitle,
                uiContentStorage.GetButtonTexture(),
                uiContentStorage.GetButtonFont(),
                new Rectangle(
                    GO_TO_MAIN_MENU_BUTTON_POSITION_X,
                    GO_TO_MAIN_MENU_BUTTON_POSITION_Y,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT));
            _goToMainMenu.OnClick += GoToMainMenuButtonClickHandler;

            _leaderBoardRecords = _dbContext.GetLeaderBoard();

            _font = uiContentStorage.GetButtonFont();
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            _goToMainMenu.Draw(_spriteBatch);
            _spriteBatch.DrawString(
                _font,
                UiResources.LeaderboardMenuTitle,
                new Vector2(LEADERBOARD_MENU_TITLE_POSITION_X, LEADERBOARD_MENU_TITLE_POSITION_Y),
                Color.White);
            DrawScoreTable();

            _spriteBatch.End();
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _goToMainMenu.Update();
        }

        private void DrawScoreTable()
        {
            DrawScoreTableHeaders();
            DrawScoreTableResults();
        }

        private void DrawScoreTableHeaders()
        {
            var offsetY = GetRowVerticalPositionOffset(SCORE_TABLE_HEADERS_ROW_OFFSET_Y);
            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableNumberColumnTitle,
                new Vector2(PLAYER_NUMBER_TITLE_POSITION_X, offsetY),
                _tableHeaderColor);
            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableNickColumnTitle,
                new Vector2(PLAYER_NICKNAME_TITLE_POSITION_X, offsetY),
                _tableHeaderColor);
            var scoreCellPosition = GetScoreCellPosition(SCORE_TABLE_HEADERS_ROW_OFFSET_Y);
            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableScoreColumnTitle,
                scoreCellPosition,
                _tableHeaderColor);
        }

        private void DrawScoreTableNickCell(string nickName, int indexY)
        {
            var offsetY = GetRowVerticalPositionOffset(indexY);
            _spriteBatch.DrawString(
                _font,
                nickName,
                GetNickCellPosition(offsetY),
                _tableResultsColor);
        }

        private void DrawScoreTableNumberCell(uint index, int indexY, Color color)
        {
            var offsetY = GetRowVerticalPositionOffset(indexY);
            _spriteBatch.DrawString(
                _font,
                index.ToString(),
                new Vector2(PLAYER_NUMBER_TITLE_POSITION_X, offsetY),
                color);
        }

        private void DrawScoreTableResults()
        {
            for (var i = 0; i < _leaderBoardRecords.Count; i++)
            {
                var indexY = i + TABLE_RESULTS_OFFSET_Y;
                var record = _leaderBoardRecords[i];
                DrawScoreTableNumberCell(record.RatingPosition, indexY, _tableResultsColor);
                DrawScoreTableNickCell(record.NickName, indexY);
                DrawScoreTableScoreCell(record.Score, indexY);
            }
        }

        private void DrawScoreTableScoreCell(int score, int offsetY)
        {
            _spriteBatch.DrawString(
                _font,
                score.ToString(),
                GetScoreCellPosition(offsetY),
                _tableResultsColor);
        }

        private static Vector2 GetNickCellPosition(int offsetY)
        {
            return new Vector2(PLAYER_NICKNAME_TITLE_POSITION_X, offsetY);
        }

        private static int GetRowVerticalPositionOffset(int indexY)
        {
            return FIRST_TABLE_ROW_POSITION_Y + ROW_STEP * indexY;
        }

        private static Vector2 GetScoreCellPosition(int indexY)
        {
            var offsetY = GetRowVerticalPositionOffset(indexY);
            return new Vector2(PLAYER_SCORE_TITLE_POSITION_X, offsetY);
        }

        private void GoToMainMenuButtonClickHandler(object? sender, EventArgs e)
        {
            TargetScene = new TitleScreen(Game, _spriteBatch);
        }
    }
}