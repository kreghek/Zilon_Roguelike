using FluentAssertions;
using NUnit.Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

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
                    var allSchemes = (IEnumerable<object>)generic.Invoke(schemeService, null);
                    Console.WriteLine($"{schemeType} Count:{allSchemes.Count()}");
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
                    var scheme = generic.Invoke(schemeService, new[] { "test" });
                    Console.WriteLine(scheme);
                };

                actList.Add(act);
            }


            // ASSERT
            foreach (var act in actList)
            {
                act.Should().Throw<TargetInvocationException>().WithInnerException<KeyNotFoundException>();
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

            var schemeHandlerFactory = new SchemeServiceHandlerFactory(schemeLocator);

            return new SchemeService(schemeHandlerFactory);
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
                .Where(x => typeof(IScheme).IsAssignableFrom(x) &&
                x.IsClass && // Иначе выберет интерфейс IScheme
                !x.IsAbstract).ToArray();  // Иначе выберет SchemeBase
            return schemeTypes;
        }
    }
}