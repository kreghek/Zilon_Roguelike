using LightInject;

namespace Zilon.Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new ServiceContainer();
            var startup = new Startup();

            startup.ConfigureServices(container);
        }
    }
}
