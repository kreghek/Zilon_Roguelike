using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Diseases;

namespace Zilon.Core.MapGenerators.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DiseaseGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, то всегда генерируется уникальное наименование болезни.
        /// </summary>
        /// <param name="diceSeed"></param>
        /// <param name="count"></param>
        [Test]
        public void Create_DifferentDiceSeed_GenerateUniqueDiseases([Values(1, 2, 3, 4, 5)] int diceSeed,
            [Values(1, 10, 100)] int count)
        {
            // ARRANGE

            var dice = new LinearDice(diceSeed);

            var generator = new DiseaseGenerator(dice);

            // ACT

            var resultDiseases = new List<IDisease>();

            for (var i = 0; i < count; i++)
            {
                var disease = generator.Create();
                if (disease != null)
                {
                    resultDiseases.Add(disease);
                }
            }

            // ASSERT
            foreach (var disease in resultDiseases)
            {
                resultDiseases.Should().NotContain(x => x != disease && x.Name == disease.Name);
            }
        }
    }
}