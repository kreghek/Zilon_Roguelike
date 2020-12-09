using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.MapGenerators.StaticObjectFactories;

namespace Zilon.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void RegisterStaticObjectFactoringFromCore(this IServiceCollection serviceRegistry)
        {
            serviceRegistry.AddSingleton<IStaticObjectFactoryCollector>(diFactory =>
            {
                var factories = diFactory.GetServices<IStaticObjectFactory>().ToArray();
                return new StaticObjectFactoryCollector(factories);
            });

            var coreAssembly = typeof(IStaticObjectFactory).Assembly;
            var allStaticObjectFactoryTypes = ImplementationGatheringHelper.GetImplementations<IStaticObjectFactory>(coreAssembly);
            foreach (var factoryType in allStaticObjectFactoryTypes)
            {
                serviceRegistry.AddSingleton(typeof(IStaticObjectFactory), factoryType);
            }
        }
    }
}