using System;
using Moq;
using NUnit.Framework;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class ActorTests
    {
        /// <summary>
        /// Этот юнит тест нуждается в переработке.
        /// Он нужен, чтобы отловить баг с поглощением злой тыквы, которая является шлемом.
        /// </summary>
        [Test()]
        [Category("NOT_unit")]
        public void UsePropTest()
        {
            var person = new Mock<IPerson>().Object;
            var player = new Mock<IPlayer>().Object;
            var node = new Mock<IMapNode>().Object;

            var actor = new Actor(person, player, node);

            var schemeFactory = new SchemeServiceHandlerFactory(CreateSchemeLocator());
            var schemeService = new SchemeService(schemeFactory);
            var propfactory = new PropFactory(schemeService);

            var testProp = propfactory.CreateEquipment(schemeService.GetScheme<IPropScheme>("evil-pumpkin"));

            actor.UseProp(testProp);
        }

        private FileSchemeLocator CreateSchemeLocator()
        {
            var schemePath = Environment.GetEnvironmentVariable("ZILON_LIV_SCHEME_CATALOG");
            var schemeLocator = new FileSchemeLocator(schemePath);
            return schemeLocator;
        }
    }
}