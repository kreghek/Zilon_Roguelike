using CDT.LIV.MonoGameClient.ViewModels.MainScene;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LIV.MonoGameClient.Scenes
{
    class MainScene : GameSceneBase
    {
        private readonly SpriteBatch _spriteBatch;
        private SectorViewModel _sectorViewModel;

        public MainScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_sectorViewModel is null)
            {
                _sectorViewModel = new SectorViewModel(Game, _spriteBatch);
                Components.Add(_sectorViewModel);
            }
        }
    }
}
