using System.Threading.Tasks;

using LightInject;

using Zilon.Core.Tactics;

namespace Zilon.Core.MassSectorGenerator
{
    public interface ISectorValidator
    {
        Task Validate(ISector sector, Scope scopeContainer);
    }
}
