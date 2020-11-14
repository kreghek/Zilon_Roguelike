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

        ISchemeServiceHandler<TScheme> ISchemeServiceHandlerFactory.Create<TScheme>()
        {
            SchemeServiceHandler<TScheme> handler = new SchemeServiceHandler<TScheme>(_schemeLocator);

            var settings = new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Error};

            handler.JsonSerializerSettings = settings;

            return handler;
        }
    }
}