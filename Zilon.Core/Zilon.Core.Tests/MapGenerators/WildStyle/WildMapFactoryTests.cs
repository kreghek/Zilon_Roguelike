using System.Threading.Tasks;

using NUnit.Framework;

using Zilon.Core.MapGenerators.WildStyle;

namespace Zilon.Core.Tests.MapGenerators.WildStyle
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class WildMapFactoryTests
    {
        [Test()]
        public async Task CreateAsyncTest_NoExceptions()
        {
            //TODO Дооформить тест
            var map = await WildMapFactory.CreateAsync(4);
        }
    }
}