using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    public interface ISectorModalManager
    {
        void ShowContainerModal(PropTransferMachine transferMachine);
        
        void ShowInventoryModal(IActor actor);
    }
}