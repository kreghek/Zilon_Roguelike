using NUnit.Framework;
using Zilon.Core.MapGenerators.WildStyle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.Core.MapGenerators.WildStyle.Tests
{
    [TestFixture()]
    public class WildMapFactoryTests
    {
        [Test()]
        public async Task CreateAsyncTestAsync()
        {
            var map = await WildMapFactory.CreateAsync(4);
        }
    }
}