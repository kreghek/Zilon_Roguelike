using System;
using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public interface IGameLoop
    {
        Task UpdateAsync();

        IActorTaskSource[] ActorTaskSources { get; set; }

        event EventHandler Updated;
    }
}