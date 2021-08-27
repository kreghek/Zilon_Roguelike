using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class ScreenManager : DrawableGameComponent
    {
        public ScreenManager(Game game) : base(game)
        {
        }

        public IScreen? ActiveScreen { get; set; }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (ActiveScreen is not null)
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

            if (ActiveScreen.TargetScreen is not null)
            {
                ActiveScreen = ActiveScreen.TargetScreen;
                ActiveScreen.TargetScreen = null;
            }

            ActiveScreen.Update(gameTime);
        }
    }
}