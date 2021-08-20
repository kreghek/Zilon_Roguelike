using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Players;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class TransitionScreen : GameSceneBase
    {
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ITransitionPool _transitionPool;

        private bool _targetSceneInitialized;

        public TransitionScreen(Game game, SpriteBatch spriteBatch) : base(game)
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
                try
                {
                    var playerPersonSectorNode = GetPlayerSectorNode(_player);

                    if (playerPersonSectorNode != null)
                    {
                        TargetScene = new MainScreen(Game, _spriteBatch);
                        _targetSceneInitialized = true;
                    }
                }
                catch (InvalidOperationException)
                {
                    // The exception was thrown because player.Globe.SectorNodes was modified during enumeration.
                }
            }
        }

        private static ISectorNode? GetPlayerSectorNode(IPlayer player)
        {
            if (player.Globe is null)
            {
                throw new InvalidOperationException();
            }

            return (from sectorNode in player.Globe.SectorNodes
                    let sector = sectorNode.Sector
                    where sector != null
                    from actor in sector.ActorManager.Items
                    where actor.Person == player.MainPerson
                    select sectorNode).SingleOrDefault();
        }
    }
}