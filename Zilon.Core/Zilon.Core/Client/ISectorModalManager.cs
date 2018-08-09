using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    public interface ISectorModalManager
    {
        void ShowContainerModal(PropTransferMachine transferMachine);
        
        void ShowInventoryModal(IActor actor);

        void ShowPerksModal(IActor actor);
    }
}