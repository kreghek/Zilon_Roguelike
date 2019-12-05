using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех кратковременных команд, связанных с изменением состояния актёра.
    /// </summary>
    public abstract class SpecialActorCommandBase : ActorCommandBase
    {
        [ExcludeFromCodeCoverage]
        protected SpecialActorCommandBase(
            ISectorManager sectorManager,
            ISectorUiState playerState)
            : base(sectorManager, playerState)
        {

        }
    }
}