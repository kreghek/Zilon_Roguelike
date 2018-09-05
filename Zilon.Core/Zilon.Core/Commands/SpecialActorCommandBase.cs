using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех кратковременных команд, связанных с изменением состояния актёра.
    /// </summary>
    public abstract class SpecialActorCommandBase : ActorCommandBase
    {
        protected SpecialActorCommandBase(IGameLoop gameloop,
            ISectorManager sectorManager,
            IPlayerState playerState) : base(gameloop, sectorManager, playerState)
        {

        }
    }
}