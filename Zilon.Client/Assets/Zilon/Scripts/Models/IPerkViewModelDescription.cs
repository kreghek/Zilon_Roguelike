using UnityEngine;

using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.Models
{
    public interface IPerkViewModelDescription
    {
        Vector3 Position { get; }
        IPerk Perk { get; }
    }
}
