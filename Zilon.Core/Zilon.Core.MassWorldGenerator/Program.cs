using System;
using System.IO;

using LightInject;

using Zilon.CommonUtilities;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.MassWorldGenerator
{
    class Program
    {
        private static ServiceContainer _globalServiceContainer;
        private static Startup _startUp;

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var schemeCatalog = ArgumentHelper.GetProgramArgument(args, "schemeCatalog");
            var resultPath = ArgumentHelper.GetProgramArgument(args, "resultPath");

            _globalServiceContainer = new ServiceContainer();
            _startUp = new Startup(schemeCatalog);
            _startUp.RegisterServices(_globalServiceContainer);

            var startTime = DateTime.UtcNow;

            var resultFolderPreffix = startTime.ToString("yyyyMMdd-HHmmss");
            var resultFolder = Path.Combine(resultPath, resultFolderPreffix);
            Directory.CreateDirectory(resultFolder);

            var iteration = 0;
            while (true)
            {
                iteration++;

                var worldGenerator = _globalServiceContainer.GetInstance<IWorldGenerator>();

                var result = await worldGenerator.GenerateGlobeAsync();

                var resultRealmsFile = $"realm{iteration:D5}.bmp";
                var resultBranchesFile = $"branches{iteration:D5}.bmp";

                result.Globe.Save(resultFolder, resultRealmsFile, resultBranchesFile);
                Console.WriteLine($"Iteration {iteration:D5} complete");
            }
        }
    }
}