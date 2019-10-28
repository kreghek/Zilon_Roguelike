using System;
using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class DropResolverTests
    {
        [Test()]
        public void ResolveTest()
        {
            var dice = new Dice(1);
            var randomSource = new DropResolverRandomSource(dice);

            var schemeFactory = new SchemeServiceHandlerFactory(CreateSchemeLocator());
            var schemeService = new SchemeService(schemeFactory);
            var propfactory = new PropFactory(schemeService);

            var dropResolver = new DropResolver(randomSource, schemeService, propfactory);

            var dropTable = new TestDropTableScheme(1, new[] { 
                new TestDropTableRecordSubScheme{ 
                    SchemeSid = "evil-pumpkin",
                    Weight = 1
                },
                new TestDropTableRecordSubScheme{ 
                    Weight = 95
                }
            });
            var props = dropResolver.Resolve(new[] { dropTable });
        }

        private FileSchemeLocator CreateSchemeLocator()
        {
            var schemePath = Environment.GetEnvironmentVariable("ZILON_LIV_SCHEME_CATALOG");
            var schemeLocator = new FileSchemeLocator(schemePath);
            return schemeLocator;
        }
    }
}

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class DropResolverTests
    {
        [Test]
        public void GetPropsTest()
        {
            // ARRANGE

            const string testPropSchemeSid = "test-resource";

            var testResourceScheme = new PropScheme
            {
                Sid = testPropSchemeSid
            };

            var randomSourceMock = new Mock<IDropResolverRandomSource>();
            randomSourceMock.Setup(x => x.RollWeight(It.IsAny<int>()))
                .Returns(1);
            randomSourceMock.Setup(x => x.RollResourceCount(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);
            var randomSource = randomSourceMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetScheme<IPropScheme>(It.Is<string>(sid => sid == testPropSchemeSid)))
                .Returns(testResourceScheme);
            var schemeService = schemeServiceMock.Object;

            var propFactoryMock = new Mock<IPropFactory>();
            propFactoryMock.Setup(x => x.CreateResource(It.IsAny<IPropScheme>(), It.IsAny<int>()))
                .Returns<IPropScheme, int>((scheme, count) => new Resource(scheme, count));
            var propFactory = propFactoryMock.Object;

            var resolver = new DropResolver(randomSource, schemeService, propFactory);

            var testDropTableRecord = new TestDropTableRecordSubScheme
            {
                SchemeSid = testPropSchemeSid,
                Weight = 1,
                MinCount = 1,
                MaxCount = 1
            };

            var testDropTable = new TestDropTableScheme(1, testDropTableRecord);


            // ACT
            var factProps = resolver.Resolve(new[] { testDropTable });



            // ASSERT
            factProps.Length.Should().Be(1);
            factProps[0].Scheme.Should().BeSameAs(testResourceScheme);
            ((Resource)factProps[0]).Count.Should().Be(1);
        }

        /// <summary>
        /// Тест проверяет, что если для записи таблицы дропа задан дополнительный дроп,
        /// то он тоже выпадает.
        /// </summary>
        [Test]
        public void GetProps_ExtraDrop()
        {
            // ARRANGE

            const string testPropSchemeSid = "test-prop";
            const string testExtraSchemeSid = "test-extra";

            var testPropScheme = new TestPropScheme
            {
                Sid = testPropSchemeSid,
                Equip = new TestPropEquipSubScheme()
            };

            var testExtraScheme = new TestPropScheme
            {
                Sid = testExtraSchemeSid
            };

            var randomSourceMock = new Mock<IDropResolverRandomSource>();
            randomSourceMock.Setup(x => x.RollWeight(It.IsAny<int>()))
                .Returns(1);
            randomSourceMock.Setup(x => x.RollResourceCount(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);
            var randomSource = randomSourceMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetScheme<IPropScheme>(It.Is<string>(sid => sid == testPropSchemeSid)))
                .Returns(testPropScheme);
            schemeServiceMock.Setup(x => x.GetScheme<IPropScheme>(It.Is<string>(sid => sid == testExtraSchemeSid)))
                .Returns(testExtraScheme);
            var schemeService = schemeServiceMock.Object;

            var propFactoryMock = new Mock<IPropFactory>();
            propFactoryMock.Setup(x => x.CreateEquipment(It.IsAny<IPropScheme>()))
                .Returns<IPropScheme>(scheme => new Equipment(scheme, null));
            propFactoryMock.Setup(x => x.CreateResource(It.IsAny<IPropScheme>(), It.IsAny<int>()))
                .Returns<IPropScheme, int>((scheme, count) => new Resource(scheme, count));
            var propFactory = propFactoryMock.Object;

            var resolver = new DropResolver(randomSource, schemeService, propFactory);

            var testDropTableRecord = new TestDropTableRecordSubScheme
            {
                SchemeSid = testPropSchemeSid,
                Weight = 1,
                Extra = new IDropTableScheme[] {
                    new TestDropTableScheme(1, new TestDropTableRecordSubScheme{
                        SchemeSid = testExtraSchemeSid,
                        Weight = 1,
                        MinCount = 1,
                        MaxCount = 1
                    })
                }
                
            };

            var testDropTable = new TestDropTableScheme(1, testDropTableRecord);


            // ACT
            var factProps = resolver.Resolve(new[] { testDropTable });



            // ASSERT
            factProps.Length.Should().Be(2);
            factProps[0].Scheme.Should().BeSameAs(testPropScheme);
            factProps[1].Scheme.Should().BeSameAs(testExtraScheme);
            ((Resource)factProps[1]).Count.Should().Be(1);
        }
    }
}