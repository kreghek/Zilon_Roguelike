using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Players;
using Zilon.Core.World;

namespace CDT.LIV.MonoGameClient.Scenes
{
    class TransitionScene : GameSceneBase
    {
        private readonly IPlayer _player;
        private readonly ITransitionPool _transitionPool;
        private readonly SpriteBatch _spriteBatch;

        private bool _targetSceneInitialized;

        public TransitionScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            var serviceScope = ((LivGame)Game).ServiceProvider;

            _player = serviceScope.GetRequiredService<IPlayer>();
            _transitionPool = serviceScope.GetRequiredService<ITransitionPool>();
            _spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            if (!_transitionPool.CheckPersonInTransition(_player.MainPerson) && !_targetSceneInitialized)
            {
                TargetScene = new MainScene(Game, _spriteBatch);
                _targetSceneInitialized = true;
            }
        }
    }
}
