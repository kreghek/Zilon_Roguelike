using Newtonsoft.Json;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.TestCommon
{
    public class StrictSchemeServiceHandlerFactory : ISchemeServiceHandlerFactory
    {
        private readonly ISchemeLocator _schemeLocator;

        public StrictSchemeServiceHandlerFactory(ISchemeLocator schemeLocator)
        {
            _schemeLocator = schemeLocator;
        }

        ISchemeServiceHandler<TScheme> ISchemeServiceHandlerFactory.Create<TScheme>()
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
