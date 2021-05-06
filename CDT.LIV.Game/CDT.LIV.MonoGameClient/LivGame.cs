using CDT.LIV.MonoGameClient.Scenes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CDT.LIV.MonoGameClient
{
    public class LivGame : Game
    {
        private GraphicsDeviceManager? _graphics;
        private SpriteBatch? _spriteBatch;
        private readonly ServiceProvider _serviceProvider;

        public ServiceProvider ServiceProvider => _serviceProvider;

        public LivGame(ServiceProvider serviceProvider)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _serviceProvider = serviceProvider;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var sceneManager = new SceneManager(this);
            var titleScene = new TitleScene(this, _spriteBatch);
            sceneManager.ActiveScene = titleScene;

            Components.Add(sceneManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
