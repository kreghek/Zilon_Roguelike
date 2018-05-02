using System.Collections.Generic;
using Zilon.Logic.Schemes;

namespace Zilon.Logic.Services
{
    interface ISchemeService
    {
        TScheme GetScheme<TScheme>(string sid) where TScheme: class, IScheme;
        IEnumerable<TScheme> GetSchemes<TScheme>() where TScheme : class, IScheme;
    }
}
