
using CDT.LIV.MonoGameClient.ViewModels.MainScene;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LIV.MonoGameClient.Scenes
{
    class MainScene : GameSceneBase
    {
        public MainScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            var sectorViewModel = new SectorViewModel(game, spriteBatch);
            Components.Add(sectorViewModel);
        }
    }
}
