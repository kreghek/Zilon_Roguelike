namespace Zilon.Core.Schemes
{
    public class SchemeServiceHandlerFactory : ISchemeServiceHandlerFactory
    {
        private readonly ISchemeLocator _schemeLocator;

        public SchemeServiceHandlerFactory(ISchemeLocator schemeLocator)
        {
            _schemeLocator = schemeLocator;
        }

        ISchemeServiceHandler<TScheme> ISchemeServiceHandlerFactory.Create<TScheme>()
        {
            var handler = new SchemeServiceHandler<TScheme>(_schemeLocator);
            return handler;
        }
    }
}
