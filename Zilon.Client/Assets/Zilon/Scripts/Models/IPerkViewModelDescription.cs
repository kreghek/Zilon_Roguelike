using UnityEngine;

using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.Models
{
    //TODO Можно сделать generic для перков и предметов
    /// <summary>
    /// Модель перка для отображения информации, для которой важна позиция
    /// модели на экране.
    /// </summary>
    /// <remarks>
    /// Используется для отображения popup-информации для перков и предметов в инвентаре.
    /// Там позиция важна, чтобы правильно расположить popup рядом с иконкой предмета.
    /// </remarks>
    public interface IPerkViewModelDescription
    {
        Vector3 Position { get; }
        IPerk Perk { get; }
    }
}
