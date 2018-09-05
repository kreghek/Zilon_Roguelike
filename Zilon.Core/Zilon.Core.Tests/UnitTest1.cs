using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Zilon.Core.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Test() {
            var i = 0;
            ThreadPool.QueueUserWorkItem(async state =>
            {
                while (true)
                {
                    await Async();
                    if (i < 10)
                    {
                        i++;
                    }
                }
            });

            i.Should().Be(0);
        }

        private Task Async()
        {
            return Task.Run(() => Thread.Sleep(100000));
        }
    }
}
