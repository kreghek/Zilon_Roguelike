using System;
using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using Zilon.Core.Diseases;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Tests.Persons.TestCases;
using Zilon.Core.World;

namespace Zilon.Core.Tests.PersonModules
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DiseaseModuleTests
    {
        [Test]
        [TestCaseSource(typeof(DiseaseDataTestCaseSource), nameof(DiseaseDataTestCaseSource.TestCases))]
        public void Update_AllProcess_ConditionAddedAndRemoved(DiseaseSymptom[] symptoms)
        {
            if (symptoms is null)
            {
                throw new ArgumentNullException(nameof(symptoms));
            }

            // ARRANGE
            var effectList = new List<IPersonEffect>();

            var сonditionModuleMock = new Mock<IConditionModule>();
            сonditionModuleMock.Setup(x => x.Add(It.IsAny<IPersonEffect>()))
                .Callback<IPersonEffect>(x => effectList.Add(x));
            сonditionModuleMock.Setup(x => x.Remove(It.IsAny<IPersonEffect>()))
                .Callback<IPersonEffect>(x => effectList.Remove(x));
            сonditionModuleMock.SetupGet(x => x.Items).Returns(effectList);
            var сonditionModule = сonditionModuleMock.Object;

            var diseaseMock = new Mock<IDisease>();
            diseaseMock.Setup(x => x.GetSymptoms())
                .Returns(symptoms);
            diseaseMock.SetupGet(x => x.ProgressSpeed)
                .Returns(1f / GlobeMetrics.OneIterationLength);
            var disease = diseaseMock.Object;

            var diseaseData = new DiseaseModule(сonditionModule);

            diseaseData.Infect(disease);

            // ACT

            for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
            {
                diseaseData.Update();
            }

            // ARRANGE
            var exceptedTimes = symptoms.Length;
            сonditionModuleMock.Verify(x => x.Add(It.Is<IPersonEffect>(effect => IsDeaseSymptom(effect))),
                Times.Exactly(exceptedTimes));
            сonditionModuleMock.Verify(x => x.Remove(It.Is<IPersonEffect>(effect => IsDeaseSymptom(effect))),
                Times.Exactly(exceptedTimes));
        }

        private static bool IsDeaseSymptom(IPersonEffect x)
        {
            return x is DiseaseSymptomEffect;
        }
    }
}