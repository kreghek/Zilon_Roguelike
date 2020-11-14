using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Schemes
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [Category(TestCategories.REAL_RESOURCE)]
    public class SchemeServiceTests
    {
        /// <summary>
        /// Тест проверяет получение схем всех типов.
        /// </summary>
        [Test]
        public void GetSchemes_AllSchemes_NoExceptions()
        {
            //ARRANGE
            var schemeTypes = ExtractSchemeTypes();

            var schemeService = CreateSchemeService();

            var actList = new List<Action>();

            // ACT

            foreach (var schemeType in schemeTypes)
            {
                // ReSharper disable once ConvertToLocalFunction
                Action act = () =>
                {
                    var method = typeof(SchemeService).GetMethod(nameof(ISchemeService.GetSchemes));
                    if (method == null)
                    {
                        throw new InvalidOperationException(
                            $"Для класса {nameof(SchemeService)} не найден метод {nameof(ISchemeService.GetSchemes)}.");
                    }

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
            //ARRANGE
            var schemeTypes = ExtractSchemeTypes();

            var schemeService = CreateSchemeService();

            var actList = new List<Action>();

            // ACT

            foreach (var schemeType in schemeTypes)
            {
                // ReSharper disable once ConvertToLocalFunction
                Action act = () =>
                {
                    var method = typeof(SchemeService).GetMethod(nameof(SchemeService.GetScheme));
                    if (method == null)
                    {
                        throw new InvalidOperationException(
                            $"Для класса {nameof(SchemeService)} не найден метод {nameof(ISchemeService.GetScheme)}.");
                    }

                    var generic = method.MakeGenericMethod(schemeType);
                    var scheme = generic.Invoke(schemeService, new object[] { "test" });
                    Console.WriteLine(scheme);
                };

                actList.Add(act);
            }


            // ASSERT
            foreach (var act in actList)
            {
                act.Should().Throw<TargetInvocationException>()
                    .WithInnerException<InvalidOperationException>()
                    .WithInnerException<KeyNotFoundException>();
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

        /// <summary>
        /// Тест проверяет, что все схемы согласованы.
        /// </summary>
        //TODO Лучше сделать отдельное консольное приложение для валидации всех схем.
        [Test]
        [Category("validation")]
        public void SchemeValidator()
        {
            // ARRANGE
            var schemeService = CreateSchemeService();

            // ACT
            var props = schemeService.GetSchemes<IPropScheme>();

            // ASSERT
            foreach (var prop in props)
            {
                if (prop.IsMimicFor != null)
                {
                    var realScheme = schemeService.GetScheme<IPropScheme>(prop.IsMimicFor);
                    realScheme.Should().NotBeNull();
                }
            }
        }

        /// <summary>
        /// Тест проверяем схемы дропа.
        /// 1. Sid предметов должен быть корректным. Эти предметы должны существовать.
        /// </summary>
        [Test]
        public void DropTables_CheckPropSids_AllPropsExist()
        {
            // ARRANGE
            var schemeService = CreateSchemeService();
            var dropTables = schemeService.GetSchemes<IDropTableScheme>();


            // ASSERT
            foreach (var dropTable in dropTables)
            {
                CheckDropTableScheme(dropTable, schemeService);
            }
        }

        private static ISchemeService CreateSchemeService()
        {
            var schemeLocator = FileSchemeLocator.CreateFromEnvVariable();

            var schemeHandlerFactory = new StrictSchemeServiceHandlerFactory(schemeLocator);

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
                            x.IsInterface && x != typeof(IScheme)).ToArray();
            return schemeTypes;
        }

        private static void CheckDropTableScheme(IDropTableScheme dropTableScheme, ISchemeService schemeService)
        {
            Action act = () =>
            {
                foreach (var record in dropTableScheme.Records)
                {
                    var propSid = record.SchemeSid;
                    if (propSid == null)
                    {
                        continue;
                    }

                    schemeService.GetScheme<IPropScheme>(propSid);
                }
            };

            act.Should().NotThrow();
        }
    }
}