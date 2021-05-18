using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LAST.MonoGameClient.Scenes
{
    internal class TitleScene : GameSceneBase
    {
        private readonly GlobeGenerationScene _globeGenerationScene;
        private readonly SpriteBatch _spriteBatch;

        public TitleScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            _globeGenerationScene = new GlobeGenerationScene(game, spriteBatch);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            var font = Game.Content.Load<SpriteFont>("Fonts/Main");

            _spriteBatch.DrawString(font, "Title", new Vector2(100, 100), Color.White);

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

            if (state.IsKeyDown(Keys.Up))
            {
                TargetScene = _globeGenerationScene;
            }
        }
    }
}