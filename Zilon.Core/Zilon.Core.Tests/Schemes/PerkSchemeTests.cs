using FluentAssertions;

using Newtonsoft.Json;

using NUnit.Framework;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Schemes
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PerkSchemeTests
    {
        /// <summary>
        /// Тест проверяет, что работы корректно десериализуется.
        /// </summary>
        [Test]
        public void Deserialization_JobsInLevel_TypeAndValueAreCorrect()
        {
            // ARRANGE
            var sourceText =
                "{\r\n  \"Name\": {\r\n    \"Ru\": \"Тест\"\r\n  },\r\n  \"Levels\":[\r\n    {\r\n\t  \"Rules\": [\r\n\t    {\r\n\t\t  \"Type\": \"Melee\"\r\n\t\t}\r\n\t  ],\r\n\t  \"Jobs\": [\r\n\t    {\r\n\t\t  \"Type\": \"Defeats\",\r\n\t\t  \"Value\": 5\r\n\t\t}\r\n\t  ]\r\n\t}\r\n  ]\r\n}";

            // ACT
            var factPerkScheme = JsonConvert.DeserializeObject<PerkScheme>(sourceText);

            // ASSERT
            factPerkScheme.Levels[0]
                          .Jobs[0]
                          .Type.Should()
                          .Be(JobType.Defeats);
        }
    }
}