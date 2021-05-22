using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class ScreenManager : DrawableGameComponent
    {
        public ScreenManager(Game game) : base(game)
        {
        }

        public GameSceneBase? ActiveScreen { get; set; }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (ActiveScreen != null)
            {
                ActiveScreen.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ActiveScreen is null)
            {
                return;
            }

            if (ActiveScreen.TargetScene != null)
            {
                ActiveScreen = ActiveScreen.TargetScene;
                ActiveScreen.TargetScene = null;
            }

            ActiveScreen.Update(gameTime);
        }
    }
}