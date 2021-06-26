namespace CDT.LAST.MonoGameClient.Screens
{
    using System;

    using CDT.LAST.MonoGameClient.Engine;
    using CDT.LAST.MonoGameClient.Resources;

    using Engine;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Resources;

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

        private readonly TextButton _goToMainMenu;

        private readonly SpriteBatch _spriteBatch;

        private readonly IUiContentStorage _uiContentStorage;

        /// <inheritdoc />
        public LeaderBoardScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            _spriteBatch = spriteBatch;
            var serviceScope = ((LivGame)Game).ServiceProvider;
            _uiContentStorage = serviceScope.GetRequiredService<IUiContentStorage>();

            _goToMainMenu = new TextButton(
                title: UiResources.MainMenuButtonTitle,
                texture: _uiContentStorage.GetButtonTexture(),
                font: _uiContentStorage.GetButtonFont(),
                rect: new Rectangle(
                    x: GO_TO_MAIN_MENU_BUTTON_POSITION_X,
                    y: GO_TO_MAIN_MENU_BUTTON_POSITION_Y,
                    width: BUTTON_WIDTH,
                    height: BUTTON_HEIGHT));
            _goToMainMenu.OnClick += GoToMainMenuButtonClickHandler;

            //TODO: prepare leader's board table
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            _goToMainMenu.Draw(_spriteBatch);

            _spriteBatch.DrawString(
                spriteFont: _uiContentStorage.GetMenuItemFont(),
                text: UiResources.LeaderboardMenuTitle,
                position: new Vector2(x: LEADERBOARD_MENU_TITLE_POSITION_X, y: LEADERBOARD_MENU_TITLE_POSITION_Y),
                color: Color.White);
            //TODO: draw table leader board
            //TODO: draw menu to add record to leader board table 

            _spriteBatch.End();
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _goToMainMenu.Update();
        }

        private void GoToMainMenuButtonClickHandler(object? sender, EventArgs e)
        {
            TargetScene = new MainScreen(game: Game, spriteBatch: _spriteBatch);
        }
    }
}