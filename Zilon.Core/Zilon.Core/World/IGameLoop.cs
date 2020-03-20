using System;
using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public interface IGameLoop
    {
        Task UpdateAsync(Globe globe);

        event EventHandler Updated;
    }
}