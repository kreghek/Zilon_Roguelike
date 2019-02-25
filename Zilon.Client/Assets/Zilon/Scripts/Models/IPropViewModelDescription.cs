using UnityEngine;

using Zilon.Core.Props;

namespace Assets.Zilon.Scripts.Models
{
    public interface IPropViewModelDescription
    {
        Vector3 Position { get; }
        IProp Prop { get; }
    }
}
