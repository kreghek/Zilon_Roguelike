using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
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
    }
}
