namespace CDT.LAST.MonoGameClient.Screens
{
    using System;
    using System.Diagnostics;

    using CDT.LAST.MonoGameClient.Engine;
    using CDT.LAST.MonoGameClient.Resources;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Zilon.Core.Scoring;

    /// <summary>
    /// The leaderboard screen displays all the results of all played users.
    /// </summary>
    public class LeaderBoardScreen : GameSceneBase
    {
        private const int GO_TO_MAIN_MENU_BUTTON_POSITION_X = 350;

        private const int GO_TO_MAIN_MENU_BUTTON_POSITION_Y = 150;

        private const int BUTTON_WIDTH = 100;

        private const int BUTTON_HEIGHT = 20;

        private const int LEADERBOARD_MENU_TITLE_POSITION_X = 50;

        private const int LEADERBOARD_MENU_TITLE_POSITION_Y = 100;

        private const int PLAYER_NICKNAME_TITLE_POSITION_X = PLAYER_NUMBER_TITLE_POSITION_X * 3;

        private const int PLAYER_NUMBER_TITLE_POSITION_X = LEADERBOARD_MENU_TITLE_POSITION_X;

        private const int PLAYER_SCORE_TITLE_POSITION_X = PLAYER_NICKNAME_TITLE_POSITION_X * 6;

        private const int ROW_STEP = 50;

        private const int FIRST_TABLE_ROW_POSITION_Y = LEADERBOARD_MENU_TITLE_POSITION_Y + ROW_STEP * 2;

        private const int PLAYER_ROW_OFFSET_Y = 1;

        private readonly SpriteFont _font;

        private readonly TextButton _goToMainMenu;

        private readonly int _playerRowOffsetY = GetRowVerticalPositionOffset(PLAYER_ROW_OFFSET_Y);

        private readonly SpriteBatch _spriteBatch;

        private readonly IUiContentStorage _uiContentStorage;

        private const int SCORE_TABLE_HEADERS_ROW_OFFSET_Y = 0;

        private string _playerNickname = "";

        /// <inheritdoc />
        public LeaderBoardScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            game.Window.TextInput += TestInput;
            _spriteBatch = spriteBatch;
            var serviceScope = ((LivGame)Game).ServiceProvider;
            _uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();
            var scoreManager = serviceScope.GetRequiredService<IScoreManager>();
            var totalPlayerScore = 1234;

            _goToMainMenu = new TextButton(
                UiResources.MainMenuButtonTitle,
                _uiContentStorage.GetButtonTexture(),
                _uiContentStorage.GetButtonFont(),
                new Rectangle(
                    GO_TO_MAIN_MENU_BUTTON_POSITION_X,
                    GO_TO_MAIN_MENU_BUTTON_POSITION_Y,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT));
            _goToMainMenu.OnClick += GoToMainMenuButtonClickHandler;
            _font = _uiContentStorage.GetButtonFont();
        }

        private void TestInput(object? sender, TextInputEventArgs e)
        {
            var playerChar = e.Character;
            _playerNickname = $"{_playerNickname}{playerChar}";
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

        private void DrawAddPlayerRows()
        {
            uint testPlayerNumberInScoreTable = 1;
            var playerScore = 322;
            DrawScoreTablePlayerNumberCell(testPlayerNumberInScoreTable);
            DrawScoreTableNickInput();
            DrawScoreTablePlayerScoreCell(playerScore);
        }

        private void DrawScoreTable()
        {
            DrawScoreTableHeaders();
            DrawAddPlayerRows();
            DrawScoreTableResults();
        }

        private void DrawScoreTableHeaders()
        {
            var offsetY = GetRowVerticalPositionOffset(SCORE_TABLE_HEADERS_ROW_OFFSET_Y);
            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableNumberColumnTitle,
                new Vector2(PLAYER_NUMBER_TITLE_POSITION_X, offsetY),
                Color.White);
            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableNickColumnTitle,
                new Vector2(PLAYER_NICKNAME_TITLE_POSITION_X, offsetY),
                Color.White);
            var scoreCellPosition = GetScoreCellPosition(0);
            _spriteBatch.DrawString(
                _font,
                UiResources.ScoreTableScoreColumnTitle,
                scoreCellPosition,
                Color.White);
        }

        private void DrawScoreTableNickCell(string nickName, int indexY)
        {
            var offsetY = GetRowVerticalPositionOffset(indexY);
            _spriteBatch.DrawString(
                _font,
                nickName,
                GetNickCellPosition(offsetY),
                Color.White);
        }

        private void DrawScoreTableNickInput()
        {
            _spriteBatch.DrawString(
                _font,
                _playerNickname,
                GetNickCellPosition(_playerRowOffsetY),
                Color.White);
        }

        private void DrawScoreTableNumberCell(uint index, int indexY)
        {
            var offsetY = GetRowVerticalPositionOffset(indexY);
            _spriteBatch.DrawString(
                _font,
                index.ToString(),
                new Vector2(PLAYER_NUMBER_TITLE_POSITION_X, offsetY),
                Color.White);
        }

        private void DrawScoreTablePlayerNumberCell(uint index)
        {
            DrawScoreTableNumberCell(index, PLAYER_ROW_OFFSET_Y);
        }

        private void DrawScoreTablePlayerScoreCell(int playerScore)
        {
            _spriteBatch.DrawString(
                _font,
                playerScore.ToString(),
                new Vector2(PLAYER_SCORE_TITLE_POSITION_X, _playerRowOffsetY),
                Color.White);
        }

        private void DrawScoreTableResults()
        {
            var testResults = new[]
            {
                new
                {
                    Nickname = "warcru", Score = 555
                },
                new
                {
                    Nickname = "Solo", Score = 322
                }
            };
            var offsetY = 2;
            for (uint i = 0; i < testResults.Length; i++)
            {
                var indexY = (int)i + offsetY;
                var number = i + 1;
                DrawScoreTableNumberCell(number, indexY);
                DrawScoreTableNickCell(testResults[i].Nickname, indexY);
                DrawScoreTableScoreCell(testResults[i].Score, indexY);
            }
        }

        private void DrawScoreTableScoreCell(int score, int offsetY)
        {
            _spriteBatch.DrawString(
                _font,
                score.ToString(),
                GetScoreCellPosition(offsetY),
                Color.White);
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
            TargetScene = new MainScreen(Game, _spriteBatch);
        }
    }
}