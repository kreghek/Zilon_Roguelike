namespace Zilon.Core.Services
{
    using System.Collections.Generic;

    using Zilon.Core.Schemes;

    public interface ISchemeService
    {
        TScheme GetScheme<TScheme>(string sid) where TScheme: class, IScheme;
        IEnumerable<TScheme> GetSchemes<TScheme>() where TScheme : class, IScheme;
    }
}
