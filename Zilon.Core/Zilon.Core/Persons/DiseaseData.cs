using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Diseases;
using Zilon.Core.Persons.Survival;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация моделя болезней персонажа.
    /// </summary>
    public class DiseaseData : IDiseaseData
    {
        private readonly List<IDiseaseProcess> _diseases;

        public DiseaseData()
        {
            _diseases = new List<IDiseaseProcess>();
        }

        /// <inheritdoc/>
        public IEnumerable<IDiseaseProcess> Diseases { get => _diseases; }

        /// <inheritdoc/>
        public void Infect(IDisease disease)
        {
            var currentProcess = _diseases.SingleOrDefault(x => x.Disease == disease);

            if (currentProcess is null)
            {
                currentProcess = new DiseaseProcess(disease);
                _diseases.Add(currentProcess);
            }
        }

        /// <inheritdoc/>
        public void RemoveDisease(IDisease disease)
        {
            var currentProcess = _diseases.SingleOrDefault(x => x.Disease == disease);
            _diseases.Remove(currentProcess);
        }

        public void Update(IEffectCollection personEffects)
        {
            foreach (var diseaseProcess in Diseases.ToArray())
            {
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

                if (diseaseProcess.Value < 0.5f)
                {
                    // Обрабатываем нарастание силы болезни.

                    if (currentPower > 0.25f)
                    {
                        // Симптомы начинаю проявляться после 25% силы болезни.

                        // Рассчитываем количество симптомов, которые должны быть при текущей силе болезни.

                        var activeSymptomCount = (int)Math.Ceiling((currentPower - 0.25f) / symptomPowerSegment);

                        // Начинаем проверять, есть ли эффекты на все эти симптомы
                        // или добавлены ли болезни симптомов в список болезней эффектов.

                        for (var i = 0; i < activeSymptomCount; i++)
                        {
                            var currentSymptom = symptoms[i];

                            var currentSymptomEffect = personEffects.Items.OfType<DiseaseSymptomEffect>()
                                .SingleOrDefault(x => x.Symptom == currentSymptom);

                            if (currentSymptomEffect is null)
                            {
                                currentSymptomEffect = new DiseaseSymptomEffect(disease, currentSymptom);
                                personEffects.Add(currentSymptomEffect);
                            }
                            else
                            {
                                currentSymptomEffect.HoldDisease(disease);
                            }
                        }
                    }
                }
                else
                {
                    var activeSymptomCount = (int)Math.Floor(currentPower / symptomPowerSegment);

                    // Начинаем снимать все эффекты, которые за пределами количества.

                    var symptomLowerIndex = activeSymptomCount;

                    for (var i = symptomLowerIndex; i < symptoms.Length; i++)
                    {
                        var currentSymptom = symptoms[i];

                        var currentSymptomEffect = personEffects.Items.OfType<DiseaseSymptomEffect>()
                            .SingleOrDefault(x => x.Symptom == currentSymptom);

                        if (currentSymptomEffect != null)
                        {
                            currentSymptomEffect.ReleaseDisease(disease);

                            if (!currentSymptomEffect.Diseases.Any())
                            {
                                personEffects.Remove(currentSymptomEffect);
                            }
                        }
                    }
                }

                // Если процесс болезни прошёл, то удаляем болезнь из модуля персонажа.
                // Счтаетс, что он её перетерпел.
                if (diseaseProcess.Value >= 1)
                {
                    RemoveDisease(diseaseProcess.Disease);
                }
            }
        }
    }
}
