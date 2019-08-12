namespace Zilon.Core.MassWorldGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            _globalServiceContainer = new ServiceContainer();
            _startUp = new Startup();
            _startUp.RegisterServices(_globalServiceContainer);
        }
    }
}
