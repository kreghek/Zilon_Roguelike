using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Scenes
{
    internal class TitleScene : GameSceneBase
    {
        private readonly GlobeGenerationScene _globeGenerationScene;
        private readonly SpriteBatch _spriteBatch;
        private readonly Button _startButton;

        public TitleScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            _globeGenerationScene = new GlobeGenerationScene(game, spriteBatch);

            var buttonTexture = game.Content.Load<Texture2D>("Sprites/ui/button");
            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _startButton = new Button("start", buttonTexture, font, new Rectangle(150, 150, 100, 20))
            {
                Click = StartButtonClickHandler
            };
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            _startButton.Draw(_spriteBatch);

            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _spriteBatch.DrawString(font, "Title", new Vector2(50, 100), Color.White);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Poll for current keyboard state
            var state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
            {
                Game.Exit();
            }

            _startButton.Update();
        }

        private void StartButtonClickHandler()
        {
            TargetScene = _globeGenerationScene;
        }
    }
}