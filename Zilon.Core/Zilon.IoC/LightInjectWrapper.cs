using LightInject;

namespace Zilon.IoC
{
    public static class LightInjectWrapper
    {
        public static ILifetime CreateSingleton()
        {
            return new PerContainerLifetime();
        }

        public static ILifetime CreateScoped()
        {
            return new PerScopeLifetime();
        }
    }
}
