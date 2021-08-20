using System.Threading;

using Zilon.Core.Localization;

namespace CDT.LAST.MonoGameClient.Screens
{
    using System;
    using System.Text;

    using Database;

    using Engine;

    using Resources;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using Zilon.Core.ScoreResultGenerating;
    using Zilon.Core.Scoring;
    using Zilon.Core.Tactics;

    /// <summary>
    /// Scores screen to show a user's score when a character died.
    /// </summary>
    internal class ScoresScreen : GameSceneBase
    {
        private const int RESTART_BUTTON_POSITION_X = 150;

        private const int RESTART_BUTTON_POSITION_Y = SCORE_MENU_TITLE_POSITION_Y + 50;

        private const int BUTTON_WIDTH = 100;

        private const int BUTTON_HEIGHT = 20;

        private const int SCORE_MENU_TITLE_POSITION_Y = 100;

        private const int NICKNAME_MAX_LENGTH = 50;

        //TODO Use localized value
        private const string DEFAULT_PLAYER_NICK_NAME = "Безымянный бродяга";

        private readonly DbContext _dbContext;
        private readonly IDeathReasonService _deathReasonService;
        private readonly IPlayerEventLogService _eventLog;

        private readonly SpriteFont _font;

        private readonly Color _playerInfoColor = Color.Yellow;

        private readonly StringBuilder _playerNicknameSb = new();
        private readonly Scores _score;
        private readonly IScoreManager _scoreManager;
        private readonly string _scoreSummary;

        private readonly SpriteBatch _spriteBatch;

        private readonly IUiContentStorage _uiContentStorage;

        private TextButton _leaderboardScreenButton;

        /// <inheritdoc />
        public ScoresScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            game.Window.TextInput += InputNickName;
            _spriteBatch = spriteBatch;

            var serviceProvider = ((LivGame)Game).ServiceProvider;

            _scoreManager = serviceProvider.GetRequiredService<IScoreManager>();

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;

            _score = _scoreManager.Scores;

            _scoreSummary = TextSummaryHelper.CreateTextSummary(_score, langName);
            _uiContentStorage = serviceProvider.GetRequiredService<IUiContentStorage>();
            _deathReasonService = serviceProvider.GetRequiredService<IDeathReasonService>();
            _eventLog = serviceProvider.GetRequiredService<IPlayerEventLogService>();

            _dbContext = serviceProvider.GetRequiredService<DbContext>();

            _font = _uiContentStorage.GetButtonFont();

            InitButtons();

            PlayerNickname = DEFAULT_PLAYER_NICK_NAME;
        }

        private string PlayerNickname
        {
            get => _playerNicknameSb.ToString();
            set => _playerNicknameSb.Append(value);
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            DrawNickInput();

            DrawMenuButtons();

            DrawScoreSummary();

            _spriteBatch.End();
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateMenuButtons();
        }

        private bool CanInputMore()
        {
            return _playerNicknameSb.Length < NICKNAME_MAX_LENGTH;
        }

        private bool CheckEmptyNickName()
        {
            if (_playerNicknameSb.Length > 0)
            {
                return false;
            }

            return true;
        }

        private void ClearPlayerNickNameClickHandler(object? sender, EventArgs e)
        {
            _playerNicknameSb.Clear();
            CheckEmptyNickName();
        }

        private void DrawMenuButtons()
        {
            var inputSize = new Vector2(256, 32);

            var buttonPosition = new Vector2(Game.GraphicsDevice.Viewport.Bounds.Center.X - inputSize.X / 2 + 128 / 2,
                inputSize.Y + 5 + 10);
            _leaderboardScreenButton.Rect = new Rectangle(buttonPosition.ToPoint(), new Point(128, 20));
            _leaderboardScreenButton.Draw(_spriteBatch);
        }

        private void DrawNickInput()
        {
            var inputSize = new Vector2(256, 32);
            var inputRect = new Rectangle((int)(Game.GraphicsDevice.Viewport.Width - inputSize.X) / 2, 10,
                (int)inputSize.X, (int)inputSize.Y);

            _spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), inputRect, Color.Gray);

            _spriteBatch.DrawString(
                _uiContentStorage.GetScoresFont(),
                PlayerNickname,
                inputRect.Location.ToVector2(),
                _playerInfoColor);
        }

        private void DrawScoreSummary()
        {
            var baseScoreSize = _uiContentStorage.GetScoresFont().MeasureString(_score.BaseScores.ToString());
            var scoresPosition = new Vector2(Game.GraphicsDevice.Viewport.Bounds.Center.X - baseScoreSize.X / 2,
                10 + 20 + 48 + 5);
            _spriteBatch.DrawString(_uiContentStorage.GetScoresFont(), _score.BaseScores.ToString(), scoresPosition,
                Color.White);

            try
            {
                var lastEvent = _eventLog.GetPlayerEvent();
                if (lastEvent is not null)
                {
                    //TODO Use current game culture
                    var deathReasonText = _deathReasonService.GetDeathReasonSummary(lastEvent, Language.Ru);
                    if (deathReasonText is not null)
                    {
                        var fullDeathReasonText = $"Причина смерти: {deathReasonText}";
                        var deathReasonSize = _font.MeasureString(fullDeathReasonText);
                        var deathReasonPosition =
                            new Vector2(Game.GraphicsDevice.Viewport.Bounds.Center.X - deathReasonSize.X / 2,
                                scoresPosition.Y + baseScoreSize.Y + 5);
                        _spriteBatch.DrawString(_font, fullDeathReasonText, deathReasonPosition, Color.Wheat);
                    }
                }
            }
            catch
            {
                //TODO Fast safe solution
            }

            var summarySize = _uiContentStorage.GetScoresFont().MeasureString(_scoreSummary);
            var summaryPosition = new Vector2(Game.GraphicsDevice.Viewport.Bounds.Center.X - summarySize.X / 2,
                10 + 20 + 48 + 5 + baseScoreSize.Y + 5 + 20);

            _spriteBatch.DrawString(
                _font,
                _scoreSummary,
                summaryPosition,
                Color.White);
        }

        private void GoToNextScreenButtonClickHandler(object? sender, EventArgs e)
        {
            SetDefaultNickNameOnEmptyNickName();
            _dbContext.AppendScores(PlayerNickname, _scoreSummary);

            _scoreManager.ResetScores();

            TargetScene = new LeaderBoardScreen(Game, _spriteBatch);
        }

        private void InitButtons()
        {
            InitRestartButton();
        }

        private void InitRestartButton()
        {
            _leaderboardScreenButton = new TextButton(
                UiResources.LeaderBoardButtonTitle,
                _uiContentStorage.GetButtonTexture(),
                _font,
                new Rectangle(
                    RESTART_BUTTON_POSITION_X,
                    RESTART_BUTTON_POSITION_Y,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT));
            _leaderboardScreenButton.OnClick += GoToNextScreenButtonClickHandler;
        }

        private void InputNickName(object? sender, TextInputEventArgs e)
        {
            var playerChar = e.Character;

            if (CanInputMore() && IsPrintedChar(playerChar))
            {
                _playerNicknameSb.Append(playerChar);
            }

            if (CheckEmptyNickName())
            {
                return;
            }

            if (e.Key == Keys.Back)
            {
                _playerNicknameSb.Remove(_playerNicknameSb.Length - 1, 1);
            }
        }

        private static bool IsPrintedChar(char ch)
        {
            return char.IsLetterOrDigit(ch);
        }

        private void SetDefaultNickNameOnEmptyNickName()
        {
            if (_playerNicknameSb.Length > 0)
            {
                return;
            }

            PlayerNickname = DEFAULT_PLAYER_NICK_NAME;
        }

        private void UpdateMenuButtons()
        {
            _leaderboardScreenButton.Update();
        }
    }
}