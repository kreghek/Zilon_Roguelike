using UnityEngine;

using Zenject;

namespace Assets.Zilon.Scripts.Services
{
    public class ModalManagerBase: MonoBehaviour
    {
        [Inject]
        private readonly DiContainer _container;

        public GameObject WindowsParent;

        public ModalDialog ModalPrefab;

        protected T CreateWindowHandler<T>(GameObject prefab) where T : IModalWindowHandler
        {
            var modal = InstantiateModalDialog();

            var modalBody = InstantiateModalBody<T>(prefab, modal);

            modal.WindowHandler = modalBody;

            return modalBody;
        }

        protected ModalDialog InstantiateModalDialog()
        {
            var modalObj = _container.InstantiatePrefab(ModalPrefab, WindowsParent.transform);

            var modal = modalObj.GetComponent<ModalDialog>();

            return modal;
        }

        protected T InstantiateModalBody<T>(GameObject prefab, ModalDialog modal) where T : IModalWindowHandler
        {
            var parent = modal.Body.transform;

            var modalBody = _container.InstantiatePrefabForComponent<T>(prefab, parent);

            return modalBody;
        }
    }
}
