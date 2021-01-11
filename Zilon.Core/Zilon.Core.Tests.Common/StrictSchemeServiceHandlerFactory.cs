using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common
{
    public sealed class StrictSchemeServiceHandlerFactory : ISchemeServiceHandlerFactory
    {
        private readonly ISchemeLocator _schemeLocator;

        [ExcludeFromCodeCoverage]
        public StrictSchemeServiceHandlerFactory(ISchemeLocator schemeLocator)
        {
            _schemeLocator = schemeLocator;
        }

        public ISchemeServiceHandler<TScheme> Create<TScheme>() where TScheme : class, IScheme
        {
            var handler = new SchemeServiceHandler<TScheme>(_schemeLocator);

            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            };

            handler.JsonSerializerSettings = settings;

            return handler;
        }
    }
}