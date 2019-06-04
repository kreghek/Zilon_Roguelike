using Zilon.Core.Persons;
using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    /// <summary>
    /// Менеджер модальных окон.
    /// Реализация на клиенте.
    /// </summary>
    public interface ISectorModalManager
    {
        void ShowContainerModal(PropTransferMachine transferMachine);
        
        void ShowInventoryModal(IActor actor);

        void ShowPerksModal(IActor actor);

        void ShowInstructionModal();

        void ShowWinModal();

        void ShowTraderModal(CitizenPerson trader);

        void ShowQuitComfirmationModal();

        void ShowScoreModal();
    }
}