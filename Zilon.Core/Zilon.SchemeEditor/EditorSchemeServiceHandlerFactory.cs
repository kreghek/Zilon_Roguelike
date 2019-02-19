using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

using Zilon.Core.Schemes;

namespace Zilon.SchemeEditor
{
    public sealed class EditorSchemeServiceHandlerFactory : ISchemeServiceHandlerFactory
    {
        private readonly ISchemeLocator _schemeLocator;

        [ExcludeFromCodeCoverage]
        public EditorSchemeServiceHandlerFactory(ISchemeLocator schemeLocator)
        {
            _schemeLocator = schemeLocator;
        }

        [ExcludeFromCodeCoverage]
        public ISchemeServiceHandler<TScheme> Create<TScheme>() where TScheme : class, IScheme
        {
            var handler = new EditorSchemeServiceHandler<TScheme>(_schemeLocator);

            handler.JsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore
            };

            return handler;
        }
    }
}
