namespace CDT.LAST.MonoGameClient.Screens
{
    using System;

    using CDT.LAST.MonoGameClient.Engine;
    using CDT.LAST.MonoGameClient.Resources;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The leaderboard screen displays all the results of all played users.
    /// </summary>
    public class LeaderBoardScreen : GameSceneBase
    {
        private readonly TextButton _goToMainMenu;

        private readonly SpriteBatch _spriteBatch;

        /// <inheritdoc />
        public LeaderBoardScreen(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            _spriteBatch = spriteBatch;

            var buttonTexture = game.Content.Load<Texture2D>("Sprites/ui/button");
            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _goToMainMenu = new TextButton(
                title: UiResources.MainMenuButtonTitle,
                texture: buttonTexture,
                font: font,
                rect: new Rectangle(x: 350, y: 150, width: 100, height: 20));
            _goToMainMenu.OnClick += GoToMainMenuButtonClickHandler;

            //TODO: prepare leader's board table
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            _goToMainMenu.Draw(_spriteBatch);

            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _spriteBatch.DrawString(
                spriteFont: font,
                text: "Leaderboard menu",
                position: new Vector2(x: 50, y: 100),
                color: Color.White);
            //TODO: draw table leader board
            //TODO: draw menu to add record to leader board table 

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

            _goToMainMenu.Update();
        }

        private void GoToMainMenuButtonClickHandler(object? sender, EventArgs e)
        {
            TargetScene = new MainScreen(game: Game, spriteBatch: _spriteBatch);
        }
    }
}