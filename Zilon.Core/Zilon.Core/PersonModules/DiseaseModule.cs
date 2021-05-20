﻿using System;
using System.Linq;

using Zilon.Core.Diseases;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Базовая реализация моделя болезней персонажа.
    /// </summary>
    public class DiseaseModule : DiseaseModuleBase
    {
        private readonly IConditionModule _сonditionModule;

        public DiseaseModule(IConditionModule сonditionModule)
        {
            _сonditionModule = сonditionModule ?? throw new ArgumentNullException(nameof(сonditionModule));
        }

        protected override void UpdateDeseaseProcess(IDiseaseProcess diseaseProcess)
        {
            if (diseaseProcess is null)
            {
                throw new ArgumentNullException(nameof(diseaseProcess));
            }

            diseaseProcess.Update();

            // Если есть болезнь, то назначаем эффекты на симпомы этой болезни.

            // Первые 25% силы болезнь себя не проявляет.
            // Оставшиеся 75% делим на равные отрезки между всеми симптомами.
            // По мере увеличения силы болезни добавляем по эффекту на каждый симптом за каждый пройденный
            // рассчитанный отрезок на симптом.
            // Если эффект на такой симптом уже есть, то добавляем к этому симптому новую болезнь.
            // После 50% прогресса болезни (то есть когда прошёл пик силы) начинаем обратный процесс - убираем эффекты
            // за каждый симптом.
            // Если для эффекта симптома указано несколько болезней, то эффект уходит только после удаления последней болезни.

            var disease = diseaseProcess.Disease;
            var symptoms = disease.GetSymptoms();

            var currentPower = diseaseProcess.CurrentPower;

            // Рассчитываем отрезок силы на один симптом.
            var symptomPowerSegment = 0.75f / symptoms.Length;

            // 0.5 длительности обрабатываем нарастание силы болезни.
            // Затем, остаток, обрабатываем, как спад (выздоровление).
            if (diseaseProcess.Value < 0.5f)
            {
                UpdatePowerUp(_сonditionModule, disease, symptoms, currentPower, symptomPowerSegment);
            }
            else
            {
                UpdatePowerDown(_сonditionModule, disease, symptoms, currentPower, symptomPowerSegment);
            }

            // Если процесс болезни прошёл, то удаляем болезнь из модуля персонажа.
            // Счтаетс, что он её перетерпел.
            if (diseaseProcess.Value >= 1)
            {
                RemoveDisease(diseaseProcess.Disease);
            }
        }

        private static void AddDiseaseEffectForSymptom(
            IConditionModule сonditionModule,
            IDisease disease,
            DiseaseSymptom symptom)
        {
            var currentSymptomEffect = сonditionModule.Items.OfType<DiseaseSymptomEffect>()
                .SingleOrDefault(x => x.Symptom == symptom);

            if (currentSymptomEffect is null)
            {
                // При создании эффекта уже фиксируется болезнь, которая его удерживает.
                currentSymptomEffect = new DiseaseSymptomEffect(disease, symptom);
                сonditionModule.Add(currentSymptomEffect);
            }
            else
            {
                currentSymptomEffect.HoldDisease(disease);
            }
        }

        private static void RemoveDiseaseEffectForSimptom(
            IConditionModule сonditionModule,
            IDisease disease,
            DiseaseSymptom symptom)
        {
            var currentSymptomEffect = сonditionModule.Items.OfType<DiseaseSymptomEffect>()
                .SingleOrDefault(x => x.Symptom == symptom);

            if (currentSymptomEffect is null)
            {
                // Просто игнорируем этот эффект.
                // Ткущий метод может вызываться несколько раз и для симптомов, которые ушли в предыдущих итерациях.
                return;
            }

            currentSymptomEffect.ReleaseDisease(disease);

            if (!currentSymptomEffect.Diseases.Any())
            {
                сonditionModule.Remove(currentSymptomEffect);
            }
        }

        private static void UpdatePowerDown(
            IConditionModule сonditionModule,
            IDisease disease,
            DiseaseSymptom[] symptoms,
            float currentPower,
            float symptomPowerSegment)
        {
            var activeSymptomCount = (int)Math.Floor(currentPower / symptomPowerSegment);

            // Начинаем снимать все эффекты, которые за пределами количества.

            var symptomLowerIndex = activeSymptomCount;

            for (var i = symptomLowerIndex; i < symptoms.Length; i++)
            {
                var currentSymptom = symptoms[i];
                RemoveDiseaseEffectForSimptom(сonditionModule, disease, currentSymptom);
            }
        }

        private static void UpdatePowerUp(IConditionModule сonditionModule, IDisease disease, DiseaseSymptom[] symptoms,
            float currentPower, float symptomPowerSegment)
        {
            if (currentPower <= 0.25f)
            {
                // Симптомы начинаю проявляться после 25% силы болезни.
                return;
            }

            // Рассчитываем количество симптомов, которые должны быть при текущей силе болезни.

            var activeSymptomCount = (int)Math.Ceiling((currentPower - 0.25f) / symptomPowerSegment);

            // Начинаем проверять, есть ли эффекты на все эти симптомы
            // или добавлены ли болезни симптомов в список болезней эффектов.

            for (var i = 0; i < activeSymptomCount; i++)
            {
                var currentSymptom = symptoms[i];
                AddDiseaseEffectForSymptom(сonditionModule, disease, currentSymptom);
            }
        }
    }
}