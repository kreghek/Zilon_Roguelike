using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CDT.LAST.MonoGameClient.Database;
using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Scoring;

namespace CDT.LAST.MonoGameClient.Screens
{
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

        private const int PLAYER_ROW_OFFSET_Y = 1;

        private const int SCORE_TABLE_HEADERS_ROW_OFFSET_Y = 2;

        private const int ADD_PLAYER_SCORE_BUTTON_POSITION_X = GO_TO_MAIN_MENU_BUTTON_POSITION_X + BUTTON_WIDTH + OFFSET_BETWEEN_BUTTON;

        private const int OFFSET_BETWEEN_BUTTON = 100;

        private const int CLEAR_PLAYER_SCORE_BUTTON_POSITION_X =
            ADD_PLAYER_SCORE_BUTTON_POSITION_X + ADD_PLAYER_NICNKNAME_BUTTON_WIDTH + OFFSET_BETWEEN_BUTTON;

        private const int NICKNAME_MAX_LENGTH = 50;

        private const int ADD_PLAYER_NICNKNAME_BUTTON_WIDTH = BUTTON_WIDTH * 2;

        private const float PLAYER_INPUT_NICKNAME_PROMPT_POSITION_X = PLAYER_NICKNAME_TITLE_POSITION_X;

        private const int TABLE_RESULTS_OFFSET_Y = 3;

        private const uint DEFAULT_PLAYER_RATING = 1;

        private readonly TextButton _addPlayerNickname;

        private const int ADD_PLAYER_SCORE_BUTTON_POSITION_Y = GO_TO_MAIN_MENU_BUTTON_POSITION_Y;

        private readonly TextButton _clearPlayerNickname;

        private const int CLEAR_PLAYER_SCORE_BUTTON_POSITION_Y = GO_TO_MAIN_MENU_BUTTON_POSITION_Y;

        private readonly DbContext _dbContext;

        private readonly SpriteFont _font;

        private readonly TextButton _goToMainMenu;

        private readonly Color _playerInfoColor = Color.Blue;

        private readonly float _playerInputNicknamePromptPositionY = GetRowVerticalPositionOffset(PLAYER_ROW_OFFSET_Y);

        private readonly uint _playerRatingInLeaderboard;

        private readonly int _playerRowOffsetY = GetRowVerticalPositionOffset(PLAYER_ROW_OFFSET_Y);

        private readonly int _playerScore;

        private readonly SpriteBatch _spriteBatch;

        private readonly Color _tableHeaderColor = Color.White;

        private readonly Color _tableResultsColor = Color.White;

        private bool _isNeedToAddedPlayerScore = true;

        private bool _isVisibleNickNamePromt;

        private List<PlayerScore> _leaderBoardRecords;

        private string PlayerNickname => _playerNicknameSb.ToString();

        private StringBuilder _playerNicknameSb = new ();

        /// <inheritdoc />
        public LeaderBoardScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            game.Window.TextInput += InputNickName;
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            var uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();
            var scoreManager = serviceScope.GetRequiredService<IScoreManager>();

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

            _addPlayerNickname = new TextButton(
                UiResources.AddPlayerNicknameButton,
                uiContentStorage.GetButtonTexture(),
                uiContentStorage.GetButtonFont(),
                new Rectangle(
                    ADD_PLAYER_SCORE_BUTTON_POSITION_X,
                    ADD_PLAYER_SCORE_BUTTON_POSITION_Y,
                    ADD_PLAYER_NICNKNAME_BUTTON_WIDTH,
                    BUTTON_HEIGHT));
            _addPlayerNickname.OnClick += AddPlayerNickNameClickHandler;

            _clearPlayerNickname = new TextButton(
                UiResources.ClearPlayerNicknameButton,
                uiContentStorage.GetButtonTexture(),
                uiContentStorage.GetButtonFont(),
                new Rectangle(
                    CLEAR_PLAYER_SCORE_BUTTON_POSITION_X,
                    CLEAR_PLAYER_SCORE_BUTTON_POSITION_Y,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT));
            _clearPlayerNickname.OnClick += ClearPlayerNickNameClickHandler;

            _leaderBoardRecords = _dbContext.GetLeaderBoard();

            _font = uiContentStorage.GetButtonFont();

            _playerScore = scoreManager.BaseScores;
            _playerRatingInLeaderboard = GetPlayerRating();
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            _goToMainMenu.Draw(_spriteBatch);
            _addPlayerNickname.Draw(_spriteBatch);
            _clearPlayerNickname.Draw(_spriteBatch);
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

            UpdateAddNickNamePlayerButtons();
        }

        private void AddPlayerNickNameClickHandler(object? sender, EventArgs e)
        {
            if (_playerNicknameSb.Length == 0)
            {
                _isVisibleNickNamePromt = true;
            }
            else
            {
                _leaderBoardRecords = _dbContext.GetLeaderBoard();
                _isNeedToAddedPlayerScore = false;
                _addPlayerNickname.OnClick -= AddPlayerNickNameClickHandler;
                _clearPlayerNickname.OnClick -= ClearPlayerNickNameClickHandler;
            }
        }

        private void ClearPlayerNickNameClickHandler(object? sender, EventArgs e)
        {
            _playerNicknameSb.Clear();
        }

        private void DrawAddPlayerRows()
        {
            DrawPlayerInputNickNamePrompt();
            DrawScoreTablePlayerNumberCell(_playerRatingInLeaderboard);
            DrawScoreTableNickInput();
            DrawScoreTablePlayerScoreCell(_playerScore);
            DrawPlayerScoreButtons();
        }

        private void DrawPlayerInputNickNamePrompt()
        {
            if (_isVisibleNickNamePromt)
            {
                _spriteBatch.DrawString(
                    _font,
                    UiResources.PlayerInputNicknamePrompt,
                    new Vector2(PLAYER_INPUT_NICKNAME_PROMPT_POSITION_X, _playerInputNicknamePromptPositionY),
                    Color.Red);
            }
        }

        private void DrawPlayerScoreButtons()
        {
            if (!_isNeedToAddedPlayerScore)
            {
                return;
            }

            _addPlayerNickname.Draw(_spriteBatch);
            _clearPlayerNickname.Draw(_spriteBatch);
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

        private void DrawScoreTableNickInput()
        {
            _spriteBatch.DrawString(
                _font,
                PlayerNickname,
                GetNickCellPosition(_playerRowOffsetY),
                _playerInfoColor);
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

        private void DrawScoreTablePlayerNumberCell(uint index)
        {
            DrawScoreTableNumberCell(index, PLAYER_ROW_OFFSET_Y, _playerInfoColor);
        }

        private void DrawScoreTablePlayerScoreCell(int playerScore)
        {
            _spriteBatch.DrawString(
                _font,
                playerScore.ToString(),
                new Vector2(PLAYER_SCORE_TITLE_POSITION_X, _playerRowOffsetY),
                _playerInfoColor);
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

        private uint GetPlayerRating()
        {
            var leaderboardRecords = _dbContext.GetAllLeaderboardRecord();
            var playerRating = leaderboardRecords.FirstOrDefault(x => x.RatingPosition <= _playerScore);

            return playerRating?.RatingPosition ?? DEFAULT_PLAYER_RATING;
        }

        private static int GetRowVerticalPositionOffset(int indexY)
        {
            return FIRST_TABLE_ROW_POSITION_Y + (ROW_STEP * indexY);
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

        private void InputNickName(object? sender, TextInputEventArgs e)
        {
            var playerChar = e.Character;

            if (CanInputMore() && IsPrintedChar(playerChar))
            {
                _playerNicknameSb.Append(playerChar);
            }

            if (_playerNicknameSb.Length > 0)
            {
                _isVisibleNickNamePromt = false;
                if (e.Key == Keys.Back)
                    _playerNicknameSb.Remove(_playerNicknameSb.Length - 1, 1);
            }
        }

        private bool CanInputMore() => _playerNicknameSb.Length < NICKNAME_MAX_LENGTH;

        private bool IsPrintedChar(char ch)
        {
            return char.IsLetterOrDigit(ch);
        }

        private void UpdateAddNickNamePlayerButtons()
        {
            if (!_isNeedToAddedPlayerScore)
            {
                return;
            }

            _addPlayerNickname.Update();
            _clearPlayerNickname.Update();
        }
    }
}