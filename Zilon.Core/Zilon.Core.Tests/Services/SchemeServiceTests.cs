namespace Zilon.Core.Tests.Services
{
    using System;
    using System.Configuration;

    using FluentAssertions;

    using NUnit.Framework;

    using Zilon.Core.Services;
    using Zilon.Core.Services.Client;

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
                var schemeService = CreateSchemeService();
            };



            // ASSERT
            createService.Should().NotThrow();
        }

        private ISchemeService CreateSchemeService()
        {
            var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

            var schemeLocator = new FileSchemeLocator(schemePath);

            return new SchemeService(schemeLocator);
        }
    }
}
