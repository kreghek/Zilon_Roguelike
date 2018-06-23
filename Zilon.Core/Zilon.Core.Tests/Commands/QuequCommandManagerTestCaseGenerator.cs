namespace Zilon.Core.Commands.Tests
{
    using System.Collections;

    using Moq;

    using NUnit.Framework;

    class QuequCommandManagerTestCaseGenerator
    {
        public static IEnumerable PopTestCases
        {
            get
            {
                //new object[] {...} нужно, что победить params, который массив начинает разбирать
                // не как выходной тип, а как набор аргументов.
                yield return new TestCaseData(new object[] { GetOneCommand() });
                yield return new TestCaseData(new object[] { GetTwoCommands() });
            }
        }

        private static ICommand[] GetOneCommand()
        {
            return new[] { CreateFakeCommand() };
        }

        private static ICommand[] GetTwoCommands()
        {
            return new[] {
                CreateFakeCommand(),
                CreateFakeCommand()
            };
        }

        private static ICommand CreateFakeCommand()
        {
            return new Mock<ICommand>().Object;
        }
    }
}
