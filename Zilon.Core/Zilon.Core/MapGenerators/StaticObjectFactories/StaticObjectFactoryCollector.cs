namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public sealed class StaticObjectFactoryCollector : IStaticObjectFactoryCollector
    {
        private readonly IStaticObjectFactory[] _factories;

        public StaticObjectFactoryCollector(IStaticObjectFactory[] factories)
        {
            _factories = factories;
        }

        public IStaticObjectFactory[] GetFactories()
        {
            return _factories;
        }
    }
}