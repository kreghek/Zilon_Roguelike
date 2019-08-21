using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Интерфейс структуры в городском районе.
    /// Каждая структура определённым образом влияет на город.
    /// Структуры могут потреблять ресурсы города. Производить ресурсы.
    /// Например, сталелитейный завод блокирует 2 единицы рабочих, 1 - управленцев и 1 - учёных. Потребляется 10 ед энергии.
    /// Производит 3 единицы промышленности в городе. Генерирует 3 единицы антисанитарии.
    /// </summary>
    public interface ILocalityStructure
    {
        /// <summary>
        /// Наименование структуры (больница, жилой сектор, металлургический завод).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Специальное наименование. Например, больница Святого [Персонажа], Завод №1.
        /// </summary>
        string SpeciaName { get; }

        /// <summary>
        /// Потребляемые ресурсы.
        /// </summary>
        Dictionary<LocalityResource, int> RequiredResources { get; }

        /// <summary>
        /// Производимые ресурсы.
        /// </summary>
        Dictionary<LocalityResource, int> ProductResources { get; }

        ///// <summary>
        ///// Обновляет лимиты запасов города.
        ///// </summary>
        ///// <param name="localityStats"> Объект для хранения запасов. </param>
        //void UpdateLimits(LocalityStats localityStats);

        ///// <summary>
        ///// Генерирует ресурсы городу.
        ///// </summary>
        ///// <param name="localityStats"> Объект для хранения запасов. </param>
        //void GenerateResources(LocalityStats localityStats);

        ///// <summary>
        ///// Структура изымает ресурс города для обеспечения собственной работоспособности.
        ///// Если ресурса нет, структура в этой итерации не работает.
        ///// То есть не производит ресурс, не добавляет лимиты.
        ///// Все структуры города обходятся по порядку их добавления в город.
        ///// То есть у последних построенных больше шанс остаться обесточенными.
        ///// </summary>
        ///// <param name="localityStats"> Объект для хранения запасов. </param>
        //void TakeResources(LocalityStats localityStats);
    }
}
