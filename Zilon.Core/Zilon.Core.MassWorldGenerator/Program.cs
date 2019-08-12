using System;
using System.Linq;
using LightInject;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.MassWorldGenerator
{
    class Program
    {
        private static ServiceContainer _globalServiceContainer;
        private static Startup _startUp;

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var schemeCatalog = GetProgramArgument(args, "schemeCatalog");

            _globalServiceContainer = new ServiceContainer();
            _startUp = new Startup(schemeCatalog);
            _startUp.RegisterServices(_globalServiceContainer);

            while (true)
            {
                var worldGenerator = _globalServiceContainer.GetInstance<IWorldGenerator>();

                await worldGenerator.GenerateGlobeAsync();
            }
        }

        private static bool HasProgramArgument(string[] args, string testArg)
        {
            return args?.Select(x => x?.Trim().ToLowerInvariant()).Contains(testArg.ToLowerInvariant()) == true;
        }

        private static string GetProgramArgument(string[] args, string testArg)
        {
            foreach (var arg in args)
            {
                var components = arg.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (string.Equals(components[0], testArg, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (components.Length >= 2)
                    {
                        return components[1];
                    }
                }
            }

            return null;
        }
    }
}
