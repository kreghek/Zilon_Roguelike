using UnityEngine;

using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.Models
{
    /// <summary>
    /// Интерфейс вью-модели, для которой можно показывать описание.
    /// Например, через tooltip.
    /// </summary>
    public interface IActViewModelDescription
    {
        Vector3 Position { get; }
        ITacticalAct Act { get; }
    }
}
