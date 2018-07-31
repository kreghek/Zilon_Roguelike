using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех кратковременных команд, связанных с изменением состояния актёра.
    /// </summary>
    public abstract class SpecialActorCommandBase : ActorCommandBase
    {
        public SpecialActorCommandBase(ISectorManager sectorManager,
            IPlayerState playerState) : base(sectorManager, playerState)
        {

        }
    }
}