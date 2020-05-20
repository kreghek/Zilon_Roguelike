using System;
using System.Threading.Tasks;

namespace Zilon.Core.Tactics
{
    public interface IGameLoop
    {
        Task UpdateAsync();

        event EventHandler Updated;
    }
}