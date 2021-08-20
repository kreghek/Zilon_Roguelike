namespace CDT.LAST.MonoGameClient.Screens
{
    using System;
    using System.Collections.Generic;

    using Database;

    using Engine;

    using Resources;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The leaderboard screen displays all the results of all played users.
    /// </summary>
    public class LeaderBoardScreen : GameSceneBase
    {
        private const int GO_TO_MAIN_MENU_BUTTON_POSITION_Y = 150;

        private const int BUTTON_WIDTH = 100;

        private const int BUTTON_HEIGHT = 20;

        private const int LEADERBOARD_MENU_TITLE_POSITION_Y = 50;

        private const int ROW_STEP = 50;

        private const int FIRST_TABLE_ROW_POSITION_Y = GO_TO_MAIN_MENU_BUTTON_POSITION_Y + ROW_STEP;

        private const int SCORE_TABLE_HEADERS_ROW_OFFSET_Y = 2;

        private const int TABLE_RESULTS_OFFSET_Y = 3;

        private const int RANK_COLUMN_WIDTH = 30;
        private const int NAME_COLUMN_WIDTH = 150;
        private const int SCORES_COLUMN_WIDTH = 80;
        private const int TABLE_WIDTH = RANK_COLUMN_WIDTH + NAME_COLUMN_WIDTH + SCORES_COLUMN_WIDTH;
        private readonly DbContext _dbContext;

        private readonly SpriteFont _font;

        private readonly TextButton _goToMainMenuButton;

        private readonly List<PlayerScore> _leaderBoardRecords;

        private readonly SpriteBatch _spriteBatch;

        private readonly Color _tableHeaderColor = Color.White;

        private readonly Color _tableResultsColor = Color.White;
        private readonly IUiContentStorage uiContentStorage;

        /// <inheritdoc />
        public LeaderBoardScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();

            _dbContext = serviceScope.GetRequiredService<DbContext>();

            _goToMainMenuButton = new TextButton(
                UiResources.MainMenuButtonTitle,
                uiContentStorage.GetButtonTexture(),
                uiContentStorage.GetButtonFont(),
                new Rectangle(
                    0,
                    0,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT));
            _goToMainMenuButton.OnClick += GoToMainMenuButtonClickHandler;

            _leaderBoardRecords = _dbContext.GetLeaderBoard(new LeaderboardLimit(limit: 10));

            _font = uiContentStorage.GetButtonFont();
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            var scoresFont = uiContentStorage.GetScoresFont();
            var titleSize = scoresFont.MeasureString(UiResources.LeaderboardMenuTitle);

            _spriteBatch.DrawString(
                scoresFont,
                UiResources.LeaderboardMenuTitle,
                new Vector2(Game.GraphicsDevice.Viewport.Bounds.Center.X - titleSize.X / 2,
                    LEADERBOARD_MENU_TITLE_POSITION_Y),
                Color.White);

            _goToMainMenuButton.Rect = new Rectangle(Game.GraphicsDevice.Viewport.Bounds.Center.X - BUTTON_WIDTH / 2,
                LEADERBOARD_MENU_TITLE_POSITION_Y + (int)titleSize.Y + 5, BUTTON_WIDTH, BUTTON_HEIGHT);
            _goToMainMenuButton.Draw(_spriteBatch);

            DrawScoreTable(_goToMainMenuButton.Rect.Bottom);

            _spriteBatch.End();
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _goToMainMenuButton.Update();
        }

        private void DrawScoreTable(int positionY)
        {
            DrawScoreTableHeaders(positionY + 30);
            DrawScoreTableResults(positionY + 20 + 30);
        }

        private void DrawScoreTableHeaders(int positionY)
        {
            var offsetY = positionY;
            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableNumberColumnTitle,
                new Vector2(Game.GraphicsDevice.Viewport.Bounds.Center.X - TABLE_WIDTH / 2, offsetY),
                _tableHeaderColor);
            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableNickColumnTitle,
                new Vector2(Game.GraphicsDevice.Viewport.Bounds.Center.X - (TABLE_WIDTH) / 2 + RANK_COLUMN_WIDTH,
                    offsetY),
                _tableHeaderColor);

            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableScoreColumnTitle,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Bounds.Center.X - (TABLE_WIDTH) / 2 + RANK_COLUMN_WIDTH +
                    NAME_COLUMN_WIDTH, offsetY),
                _tableHeaderColor);
        }

        private void DrawScoreTableNickCell(string nickName, int positionX, int positionY)
        {
            _spriteBatch.DrawString(
                _font,
                nickName.Length > 20 ? nickName.Substring(0, 20) : nickName,
                new Vector2(positionX, positionY),
                _tableResultsColor);
        }

        private void DrawScoreTableNumberCell(int index, int positionX, int positionY)
        {
            _spriteBatch.DrawString(
                _font,
                index.ToString(),
                new Vector2(positionX, positionY),
                _tableResultsColor);
        }

        private void DrawScoreTableResults(int tablePositionY)
        {
            var positionX = Game.GraphicsDevice.Viewport.Bounds.Center.X - TABLE_WIDTH / 2;

            for (var i = 0; i < _leaderBoardRecords.Count; i++)
            {
                var positionY = tablePositionY + i * 20;
                var record = _leaderBoardRecords[i];
                var rank = i + 1; // starts with the zero
                DrawScoreTableNumberCell(rank, positionX, positionY);
                DrawScoreTableNickCell(record.NickName, positionX + RANK_COLUMN_WIDTH, positionY);
                DrawScoreTableScoreCell(record.Score, positionX + RANK_COLUMN_WIDTH + NAME_COLUMN_WIDTH, positionY);
            }
        }

        private void DrawScoreTableScoreCell(int score, int positionX, int positionY)
        {
            _spriteBatch.DrawString(
                _font,
                score.ToString(),
                new Vector2(positionX, positionY),
                _tableResultsColor);
        }

        private static int GetRowVerticalPositionOffset(int indexY)
        {
            return FIRST_TABLE_ROW_POSITION_Y + ROW_STEP * indexY;
        }

        private void GoToMainMenuButtonClickHandler(object? sender, EventArgs e)
        {
            TargetScene = new TitleScreen(Game, _spriteBatch);
        }
    }
}