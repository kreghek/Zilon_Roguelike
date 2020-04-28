using System;
using System.IO;

using Zilon.CommonUtilities;

namespace Versioner
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathToVersionFile = ArgumentHelper.GetProgramArgument(args, "VersionFilePath");
            if (string.IsNullOrWhiteSpace(pathToVersionFile))
            {
                throw new InvalidOperationException($"Version File by path \"{pathToVersionFile}\" not found");
            }

            var versionLines = File.ReadAllLines(pathToVersionFile);
            var buildNumber = GetNewBuildNumber(versionLines);

            versionLines[3] = buildNumber.ToString();

            Console.Out.WriteLine($"{versionLines[0]}.{versionLines[1]}.{versionLines[2]}-alpha.{versionLines[3]}");

            File.WriteAllLines(pathToVersionFile, versionLines);
        }

        private static int GetNewBuildNumber(string[] versionLines)
        {
            try
            {
                if (!int.TryParse(versionLines[3], out var buildNumber))
                {
                    var message = GetInvalidFormatExceptionMessage(versionLines);
                    throw new InvalidOperationException(message);
                }

                buildNumber++;

                return buildNumber;
            }
            catch (IndexOutOfRangeException)
            {
                var message = GetInvalidFormatExceptionMessage(versionLines);
                throw new InvalidOperationException(message);
            }
            catch
            {
                throw;
            }
        }

        private static string GetInvalidFormatExceptionMessage(string[] versionLines)
        {
            return $"Invalid Version File format:\n{string.Join('\n', versionLines)}.\nCorrect is:\nint\nint\nint\nint";
        }
    }
}
