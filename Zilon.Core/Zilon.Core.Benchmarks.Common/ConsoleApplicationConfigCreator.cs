using Zilon.CommonUtilities;
using Zilon.Core.Benchmark;

namespace Zilon.Core.Benchmarks.Common
{
    /// <summary>
    /// Вспомогательный класс для создания общего конфига бенчей
    /// </summary>
    public static class ConsoleApplicationConfigCreator
    {
        /// <summary>
        /// Создаёт кастомный конфиг бенчей на основе аргументов командной строки.
        /// </summary>
        /// <param name="args"> Аргументы командной строки. </param>
        /// <returns> Возвращает объект конфигурации. </returns>
        public static Config CreateBenchConfig(string[] args)
        {
            var buildNumber = ArgumentHelper.GetProgramArgument(args, "BUILD_NUMBER");
            var iterationCountString = ArgumentHelper.GetProgramArgument(args, "ITERATION_COUNT");
            var iterationCount = int.Parse(iterationCountString, System.Globalization.CultureInfo.InvariantCulture);
            var monoName = "mono";
            var monoPath = ArgumentHelper.GetProgramArgument(args, "MONO_PATH");
            var artifactPath = ArgumentHelper.GetProgramArgument(args, "ARTIFACT_PATH");

            var config = new Config(buildNumber, iterationCount, monoName, monoPath, artifactPath);

            return config;
        }
    }
}
