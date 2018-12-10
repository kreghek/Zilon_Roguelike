using FluentAssertions;

using Newtonsoft.Json;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Schemes
{
    [TestFixture]
    public class PropSchemeTests
    {
        /// <summary>
        /// Тест проверяет, что работы корректно десериализуется.
        /// </summary>
        [Test]
        public void Deserialization_ArmorsInProp_ArmorIsCorrect()
        {
            // ARRANGE

            var sourceText = @"{
""Equip"": {
  ""SlotTypes"": [ ""Body"" ],
  ""Armors"": [
  {
    ""Impact"": ""Kinetic"",
    ""AbsorbtionLevel"": ""Normal"",
    ""ArmorRank"": 5
  }]
  },
    ""Name"": {
      ""En"": ""Steel Armor"",
	  ""Ru"": ""Стальная броня""
  }
}
";



            // ACT
            var factPropScheme = JsonConvert.DeserializeObject<PropScheme>(sourceText);



            // ASSERT
            factPropScheme.Equip.Armors[0].Impact.Should().Be(ImpactType.Kinetic);
            factPropScheme.Equip.Armors[0].AbsorbtionLevel.Should().Be(PersonRuleLevel.Normal);
            factPropScheme.Equip.Armors[0].ArmorRank.Should().Be(5);
        }
    }
}
