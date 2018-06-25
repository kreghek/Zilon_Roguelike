using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.CommonServices
{
    public interface ISchemeService
    {
        TScheme GetScheme<TScheme>(string sid) where TScheme: class, IScheme;
        IEnumerable<TScheme> GetSchemes<TScheme>() where TScheme : class, IScheme;
    }
}
