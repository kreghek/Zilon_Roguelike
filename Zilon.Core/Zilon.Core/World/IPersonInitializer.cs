using System.Collections.Generic;
using System.Threading.Tasks;

using Zilon.Core.Persons;

namespace Zilon.Core.World
{
    public interface IPersonInitializer
    {
        Task<IEnumerable<IPerson>> CreateStartPersonsAsync();
    }
}