using System;
using System.Configuration;
using FluentAssertions;
using NUnit.Framework;
using Zilon.Logic.Services.Client;

namespace Zilon.Logic.Services.Tests
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
        public void Contructor_CorrectSchemes_NoExceptions()
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
