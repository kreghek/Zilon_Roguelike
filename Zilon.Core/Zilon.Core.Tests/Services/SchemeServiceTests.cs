using System;
using System.Configuration;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices;

namespace Zilon.Core.Tests.Services
{
    [TestFixture]
    public class SchemeServiceTests
    {
        /// <summary>
        /// 1. В системе есть каталог схем.
        /// 2. Создаём службу.
        /// 3. При загрузке схем не происходит ошибок.
        /// </summary>
        [Test]
        public void Constructor_CorrectSchemes_NoExceptions()
        {
            // ARRANGE



            // ACT
            Action createService = () =>
            {
                CreateSchemeService();
            };



            // ASSERT
            createService.Should().NotThrow();
        }

        private void CreateSchemeService()
        {
            var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

            var schemeLocator = new FileSchemeLocator(schemePath);

            new SchemeService(schemeLocator);
        }
    }
}
