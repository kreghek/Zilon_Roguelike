using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Schemes
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MonsterSchemeTests
    {
        /// <summary>
        /// Тест проверяет, что монстр корректно десериализуется.
        /// </summary>
        [Test]
        public void Deserialization_Defences_TypeAndValueAreCorrect()
        {
            // ARRANGE
            var sourceText = @"
    {
      ""Hp"": 5,
      ""PrimaryAct"": {
                    ""Efficient"": {
                        ""Dice"": 3,
                        ""Count"": 1
                    },
                      ""Range"": {
                                    ""Min"": 1,
                                    ""Max"": 1
                     },
                    ""Offence"": {
                                    ""Type"": ""Tactical"",
                                    ""Impact"": ""Kinetic""
                    },
        },
        ""Defense"": {
                    ""Defenses"": [
                        {
                        ""Type"": ""TacticalDefence"",
                        ""Level"": ""Lesser""
                        }
                    ]
        },
        ""DropTableSids"": [ ""survival"" ]
}
";


            // ACT
            var factPerkScheme = JsonConvert.DeserializeObject<MonsterScheme>(sourceText);


            // ASSERT
            factPerkScheme.Defense.Defenses[0].Type.Should().Be(DefenceType.TacticalDefence);
        }
    }
}