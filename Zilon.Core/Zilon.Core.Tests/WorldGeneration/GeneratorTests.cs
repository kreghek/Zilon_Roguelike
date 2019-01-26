using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.Tests
{
    [TestFixture()]
    public class GeneratorTests
    {
        [Test()]
        public void GenerateTest()
        {
            var dice = new Dice();
            var generator = new Generator(dice);

            var globe = generator.Generate();
            globe.Save(@"c:\worldgen");
        }
    }
}