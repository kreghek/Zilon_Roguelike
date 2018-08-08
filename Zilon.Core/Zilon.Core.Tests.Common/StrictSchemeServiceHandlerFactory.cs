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
            var handler = new StrictSchemeServiceHandler<TScheme>(_schemeLocator);
            return handler;
        }
    }
}
