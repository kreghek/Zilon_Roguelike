using UnityEngine;
using Zenject;
using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class SectorModalManager : MonoBehaviour, ISectorModalManager
	{
		// ReSharper disable UnassignedField.Global
		// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649
		public GameObject WindowsParent;

		public ModalDialog ModalPrefab;

		public ContainerModalBody ContainerModalPrefab;

		public InventoryModalBody InventoryModalPrefab;
		
		public PerksModalBody PerksModalPrefab;

        public InstructionModalBody InstructionModalPrefab;

        public WinModalBody WinModalPrefab;

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnassignedField.Global

        [Inject] 
		private DiContainer _container;

#pragma warning restore 649
        // ReSharper disable once UnusedMember.Global

        public void ShowContainerModal(PropTransferMachine transferMachine)
		{
			var modalBody = CreateWindowHandler<ContainerModalBody>(ContainerModalPrefab.gameObject);

			modalBody.Init(transferMachine);
		}

		public void ShowInventoryModal(IActor actor)
		{
			var modalBody = CreateWindowHandler<InventoryModalBody>(InventoryModalPrefab.gameObject);

			modalBody.Init(actor);
		}

		public void ShowPerksModal(IActor actor)
		{
			var modalBody = CreateWindowHandler<PerksModalBody>(PerksModalPrefab.gameObject);

			modalBody.Init(actor);
		}

        public void ShowInstructionModal()
        {
            CreateWindowHandler<InstructionModalBody>(InstructionModalPrefab.gameObject);
        }

        public void ShowWinModal()
        {
            CreateWindowHandler<WinModalBody>(WinModalPrefab.gameObject);
        }

        private T CreateWindowHandler<T>(GameObject prefab) where T : IModalWindowHandler
		{
			var modal = InstantiateModalDialog();

			var modalBody = InstantiateModalBody<T>(prefab, modal);

			modal.WindowHandler = modalBody;

			return modalBody;
		}

		private ModalDialog InstantiateModalDialog()
		{
			var modalObj = _container.InstantiatePrefab(ModalPrefab, WindowsParent.transform);

			var modal = modalObj.GetComponent<ModalDialog>();

			return modal;
		}

		private T InstantiateModalBody<T>(GameObject prefab, ModalDialog modal) where T : IModalWindowHandler
		{
			var parent = modal.Body.transform;

			var modalBody = _container.InstantiatePrefabForComponent<T>(prefab, parent);

			return modalBody;
		}
	}
}
