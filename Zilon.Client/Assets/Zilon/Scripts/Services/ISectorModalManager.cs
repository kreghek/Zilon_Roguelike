using UnityEngine;
using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.Services
{
    public interface ISectorModalManager
    {
        void ShowContainerModal(IProp[] props);
    }
}