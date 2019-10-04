using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    /// <summary>
    /// Менеджер модальных окон для сектора.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SectorModalManager : CommonModalManagerBase, ISectorModalManager, ICommonModalManager
    {
        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649
        public ContainerModalBody ContainerModalPrefab;

        public InventoryModalBody InventoryModalPrefab;

        public PerksModalBody PersonModalPrefab;

        public InstructionModalBody InstructionModalPrefab;

        public TraderModalBody TraderModalPrefab;

        public DialogModalBody DialogModalPrefab;

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnassignedField.Global

#pragma warning restore 649

        public void ShowContainerModal(PropTransferMachine transferMachine)
        {
            var modalBody = CreateWindowHandler<ContainerModalBody>(ContainerModalPrefab.gameObject);

            modalBody.Init(transferMachine);
        }

        public void ShowInventoryModal(IActor actor)
        {
            var modalBody = CreateWindowHandler<InventoryModalBody>(InventoryModalPrefab.gameObject);

            modalBody.Init();
        }

        public void ShowPerksModal(IActor actor)
        {
            var modalBody = CreateWindowHandler<PerksModalBody>(PersonModalPrefab.gameObject);

            modalBody.Init(actor);
        }

        public void ShowInstructionModal()
        {
            CreateWindowHandler<InstructionModalBody>(InstructionModalPrefab.gameObject);
        }

        public void ShowTraderModal(CitizenPerson trader)
        {
            var modalBody = CreateWindowHandler<TraderModalBody>(TraderModalPrefab.gameObject);
            modalBody.Init(trader);
        }

        public void ShowDialogModal(CitizenPerson citizen)
        {
            var modalBody = CreateWindowHandler<DialogModalBody>(DialogModalPrefab.gameObject);
            modalBody.Init(citizen);
        }
    }
}
