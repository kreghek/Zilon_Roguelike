using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Schemes
{
    public sealed class SchemeServiceHandlerFactory : ISchemeServiceHandlerFactory
    {
        private readonly ISchemeLocator _schemeLocator;

        [ExcludeFromCodeCoverage]
        public SchemeServiceHandlerFactory(ISchemeLocator schemeLocator)
        {
            _schemeLocator = schemeLocator;
        }

        [ExcludeFromCodeCoverage]
        public ISchemeServiceHandler<TScheme> Create<TScheme>() where TScheme : class, IScheme
        {
            var handler = new SchemeServiceHandler<TScheme>(_schemeLocator);
            return handler;
        }
    }
}