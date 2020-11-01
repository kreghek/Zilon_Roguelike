using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        ///     Тест проверяет получение схем всех типов.
        /// </summary>
        [Test]
        public void GetSchemes_AllSchemes_NoExceptions()
        {
            //ARRANGE
            Type[] schemeTypes = ExtractSchemeTypes();

            ISchemeService schemeService = CreateSchemeService();

            List<Action> actList = new List<Action>();

            // ACT

            foreach (Type schemeType in schemeTypes)
            {
                // ReSharper disable once ConvertToLocalFunction
                Action act = () =>
                {
                    MethodInfo? method = typeof(SchemeService).GetMethod(nameof(ISchemeService.GetSchemes));
                    if (method == null)
                    {
                        throw new InvalidOperationException(
                            $"Для класса {nameof(SchemeService)} не найден метод {nameof(ISchemeService.GetSchemes)}.");
                    }

                    MethodInfo generic = method.MakeGenericMethod(schemeType);
                    IEnumerable<object> allSchemes = (IEnumerable<object>)generic.Invoke(schemeService, null);
                    Console.WriteLine($"{schemeType} Count:{allSchemes.Count()}");
                };

                actList.Add(act);
            }


            // ASSERT
            foreach (Action act in actList)
            {
                act.Should().NotThrow();
            }
        }

        /// <summary>
        ///     Тест проверяет получение схем всех типов.
        /// </summary>
        [Test]
        public void GetScheme_OneScheme_NoExceptions()
        {
            //ARRANGE
            Type[] schemeTypes = ExtractSchemeTypes();

            ISchemeService schemeService = CreateSchemeService();

            List<Action> actList = new List<Action>();

            // ACT

            foreach (Type schemeType in schemeTypes)
            {
                // ReSharper disable once ConvertToLocalFunction
                Action act = () =>
                {
                    MethodInfo? method = typeof(SchemeService).GetMethod(nameof(SchemeService.GetScheme));
                    if (method == null)
                    {
                        throw new InvalidOperationException(
                            $"Для класса {nameof(SchemeService)} не найден метод {nameof(ISchemeService.GetScheme)}.");
                    }

                    MethodInfo generic = method.MakeGenericMethod(schemeType);
                    object? scheme = generic.Invoke(schemeService, new object[] {"test"});
                    Console.WriteLine(scheme);
                };

                actList.Add(act);
            }


            // ASSERT
            foreach (Action act in actList)
            {
                act.Should().Throw<TargetInvocationException>()
                    .WithInnerException<InvalidOperationException>()
                    .WithInnerException<KeyNotFoundException>();
            }
        }

        /// <summary>
        ///     1. В системе есть каталог схем.
        ///     2. Создаём службу.
        ///     3. При загрузке схем не происходит ошибок.
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
        ///     Тест проверяет, что все схемы согласованы.
        /// </summary>
        //TODO Лучше сделать отдельное консольное приложение для валидации всех схем.
        [Test]
        [Category("validation")]
        public void SchemeValidator()
        {
            // ARRANGE
            ISchemeService schemeService = CreateSchemeService();

            // ACT
            IPropScheme[] props = schemeService.GetSchemes<IPropScheme>();

            // ASSERT
            foreach (IPropScheme prop in props)
            {
                if (prop.IsMimicFor != null)
                {
                    IPropScheme realScheme = schemeService.GetScheme<IPropScheme>(prop.IsMimicFor);
                    realScheme.Should().NotBeNull();
                }
            }
        }

        /// <summary>
        ///     Тест проверяем схемы дропа.
        ///     1. Sid предметов должен быть корректным. Эти предметы должны существовать.
        /// </summary>
        [Test]
        public void DropTables_CheckPropSids_AllPropsExist()
        {
            // ARRANGE
            ISchemeService schemeService = CreateSchemeService();
            IDropTableScheme[] dropTables = schemeService.GetSchemes<IDropTableScheme>();


            // ASSERT
            foreach (IDropTableScheme dropTable in dropTables)
            {
                CheckDropTableScheme(dropTable, schemeService);
            }
        }

        private static ISchemeService CreateSchemeService()
        {
            FileSchemeLocator schemeLocator = FileSchemeLocator.CreateFromEnvVariable();

            StrictSchemeServiceHandlerFactory schemeHandlerFactory =
                new StrictSchemeServiceHandlerFactory(schemeLocator);

            return new SchemeService(schemeHandlerFactory);
        }

        /// <summary>
        ///     Извлекает все схемы из корневой сборки.
        /// </summary>
        /// <returns> Типы схем. </returns>
        private static Type[] ExtractSchemeTypes()
        {
            Assembly assembly = typeof(IScheme).Assembly;
            Type[] allTypes = assembly.GetTypes();
            Type[] schemeTypes = allTypes
                .Where(x => typeof(IScheme).IsAssignableFrom(x) &&
                            x.IsInterface && x != typeof(IScheme)).ToArray();
            return schemeTypes;
        }

        private static void CheckDropTableScheme(IDropTableScheme dropTableScheme, ISchemeService schemeService)
        {
            Action act = () =>
            {
                foreach (IDropTableRecordSubScheme record in dropTableScheme.Records)
                {
                    string propSid = record.SchemeSid;
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