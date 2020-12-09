using System;

using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    /// <summary>
    /// Base implementation of <see cref="IStaticObjectFactoryCollector"/>.
    /// </summary>
    public sealed class StaticObjectFactoryCollector : IStaticObjectFactoryCollector
    {
        private readonly IStaticObjectFactory[] _factories;

        public StaticObjectFactoryCollector(IStaticObjectFactory[] factories)
        {
            _factories = factories;
        }

        /// <inheritdoc/>
        public IStaticObjectFactory SelectFactoryByStaticObjectPurpose(PropContainerPurpose purpose)
        {
            foreach (var factory in _factories)
            {
                if (factory.Purpose != purpose)
                {
                    continue;
                }

                return factory;
            }

            throw new InvalidOperationException($"Not found factory for purpose {purpose}.");
        }
    }
}