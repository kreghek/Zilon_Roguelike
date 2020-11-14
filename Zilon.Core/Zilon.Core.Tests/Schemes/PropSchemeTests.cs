using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Schemes
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PropSchemeTests
    {
        /// <summary>
        ///     Тест проверяет, что показания брони корректно десериализуется.
        /// </summary>
        [Test]
        public void Deserialization_ArmorsInProp_ArmorIsCorrect()
        {
            // ARRANGE

            string sourceText = @"{
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

        /// <summary>
        ///     Тест проверяет, что работы корректно десериализуется.
        /// </summary>
        [Test]
        public void Deserialization_TagsEquip()
        {
            // ARRANGE

            string sourceText = @"{
  ""Tags"": [ ""weapon"", ""ranged"" ],
  ""Equip"": {
                ""ActSids"": [
                  ""shot""
    ],
    ""SlotTypes"": [ ""Hand"" ]
    },
  ""Name"": {
    ""Ru"": ""Пистолет""
  }
}
";


            // ACT
            var factPropScheme = JsonConvert.DeserializeObject<PropScheme>(sourceText);


            // ASSERT
            factPropScheme.Tags.Should().BeEquivalentTo("weapon", "ranged");
            factPropScheme.Equip.Should().NotBeNull();
        }
    }
}