using Microsoft.Xna.Framework;

namespace CDT.LIV.MonoGameClient
{
    class SceneManager : DrawableGameComponent
    {
        public SceneManager(Game game) : base(game)
        {
        }

        public GameSceneBase? ActiveScene { get; set; }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (ActiveScene != null)
            {
                ActiveScene.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ActiveScene is null)
            {
                return;
            }

            if (ActiveScene.TargetScene != null)
            {
                ActiveScene = ActiveScene.TargetScene;
                ActiveScene.TargetScene = null;
            }

            ActiveScene.Update(gameTime);
        }
    }
}
