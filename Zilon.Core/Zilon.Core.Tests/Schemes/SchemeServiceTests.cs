using FluentAssertions;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Zilon.Core.Schemes.Tests
{
    [TestFixture]
    public class SchemeServiceTests
    {
        /// <summary>
        /// Тест проверяет получение схем всех типов.
        /// </summary>
        [Test]
        public void GetSchemes_AllSchemes_NoExceptions()
        {
            //ARRAGE
            var schemeTypes = ExtractSchemeTypes();

            var schemeService = CreateSchemeService();

            var actList = new List<Action>();

            // ACT

            foreach (var schemeType in schemeTypes)
            {


                Action act = () =>
                {
                    var method = typeof(SchemeService).GetMethod(nameof(SchemeService.GetSchemes));
                    var generic = method.MakeGenericMethod(schemeType);
                    var allSchemes = generic.Invoke(schemeService, null);
                };

                actList.Add(act);
            }


            // ASSERT
            foreach (var act in actList)
            {
                act.Should().NotThrow();
            }
        }

        /// <summary>
        /// Тест проверяет получение схем всех типов.
        /// </summary>
        [Test]
        [Ignore("Не рабоатает. Всегда проходит.")]
        public void GetScheme_OneScheme_NoExceptions()
        {
            //ARRAGE
            var schemeTypes = ExtractSchemeTypes();

            var schemeService = CreateSchemeService();

            var actList = new List<Action>();

            // ACT

            foreach (var schemeType in schemeTypes)
            {


                Action act = () =>
                {
                    var method = typeof(SchemeService).GetMethod(nameof(SchemeService.GetScheme));
                    var generic = method.MakeGenericMethod(schemeType);
                    var scheme = generic.Invoke(schemeService, new[] { "default" });
                };

                actList.Add(act);
            }


            // ASSERT
            foreach (var act in actList)
            {
                act.Should().NotThrow();
            }
        }

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

        private SchemeService CreateSchemeService()
        {
            var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

            var schemeLocator = new FileSchemeLocator(schemePath);

            return new SchemeService(schemeLocator);
        }

        /// <summary>
        /// Извлекает все схемы из корневой сборки.
        /// </summary>
        /// <returns> Типы схем. </returns>
        private static Type[] ExtractSchemeTypes()
        {
            var assembly = typeof(IScheme).Assembly;
            var allTypes = assembly.GetTypes();
            var schemeTypes = allTypes
                .Where(x => x.IsAssignableFrom(typeof(IScheme)) &&
                x.IsClass && // Иначе выберет интерфейс IScheme
                !x.IsAbstract).ToArray();  // Иначе выберет SchemeBase
            return schemeTypes;
        }
    }
}