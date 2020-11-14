using System;
using System.Collections.Generic;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Diseases;

namespace Zilon.Core.MapGenerators.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DiseaseGeneratorTests
    {
        /// <summary>
        ///     Тест проверяет, то всегда генерируется уникальное наименование болезни.
        /// </summary>
        /// <param name="diceSeed"></param>
        /// <param name="count"></param>
        [Test]
        public void Create_DifferentDiceSeed_GenerateUniqueDiseases([Values(1, 2, 3, 4, 5)] int diceSeed,
            [Values(1, 10, 100)] int count)
        {
            // ARRANGE

            LinearDice dice = new LinearDice(diceSeed);

            DiseaseGenerator generator = new DiseaseGenerator(dice);

            // ACT

            List<IDisease> resultDiseases = new List<IDisease>();

            for (int i = 0; i < count; i++)
            {
                IDisease disease = generator.Create();
                if (disease != null)
                {
                    resultDiseases.Add(disease);
                    Console.WriteLine(
                        $"{disease.Name.Secondary?.Ru} {disease.Name.PrimaryPrefix?.Ru}{disease.Name.Primary?.Ru} {disease.Name.Subject?.Ru}");
                }
            }

            // ASSERT
            foreach (IDisease disease in resultDiseases)
            {
                resultDiseases.Should().NotContain(x => x != disease && x.Name == disease.Name);
            }
        }
    }
}