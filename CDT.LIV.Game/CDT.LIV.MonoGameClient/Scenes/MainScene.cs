using CDT.LIV.MonoGameClient.ViewModels.MainScene;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.Scenes
{
    class MainScene : GameSceneBase
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly ISectorUiState _uiState;
        private SectorViewModel? _sectorViewModel;
        private Camera? _camera;

        public MainScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;
            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_sectorViewModel is null)
            {
                _camera = new Camera();

                _sectorViewModel = new SectorViewModel(Game, _camera, _spriteBatch);

                Components.Add(_sectorViewModel);
            }

            if (_camera != null && _uiState.ActiveActor != null)
            {
                _camera.Follow(_uiState.ActiveActor, Game);
            }
        }
    }

    public class Camera
    {
        private const int UNIT_SIZE = 50;

        public Matrix Transform { get; private set; }

        public void Follow(IActorViewModel target, Game game)
        {
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)(target.Actor.Node)).OffsetCoords);

            var position = Matrix.CreateTranslation(
              -playerActorWorldCoords[0] * UNIT_SIZE,
              -playerActorWorldCoords[1] * UNIT_SIZE / 2,
              0);

            var offset = Matrix.CreateTranslation(
                game.GraphicsDevice.Viewport.Width / 2,
                game.GraphicsDevice.Viewport.Height / 2,
                0);

            Transform = position * offset;
        }
    }
}
