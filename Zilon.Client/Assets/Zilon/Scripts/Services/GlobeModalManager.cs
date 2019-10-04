﻿using UnityEngine;

using Zenject;

using Zilon.Core.Client.Windows;

namespace Assets.Zilon.Scripts.Services
{
    /// <summary>
    /// Менеджер модальных окон для глобальной карты.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GlobeModalManager : MonoBehaviour, IGlobeModalManager
    {
        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649
        public GameObject WindowsParent;

        public ModalDialog ModalPrefab;

        public QuitModalBody QuitModalPrefab;

        public ScoreModalBody ScoreModalPrefab;

        public HistoryModalBody HistoryModalPrefab;

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnassignedField.Global

        [Inject]
        private DiContainer _container;

#pragma warning restore 649
        // ReSharper disable once UnusedMember.Global

        public void ShowQuitComfirmationModal()
        {
            ShowQuitComfirmationModalInner("Quit game?", closeGame: true);
        }

        public void ShowQuitTitleComfirmationModal()
        {
            ShowQuitComfirmationModalInner("End game session?", closeGame: false);
        }

        public void ShowScoreModal()
        {
            var modalBody = CreateWindowHandler<ScoreModalBody>(ScoreModalPrefab.gameObject);
            modalBody.Init();
        }

        public void ShowHistoryBookModal()
        {
            var modalBody = CreateWindowHandler<HistoryModalBody>(HistoryModalPrefab.gameObject);
            modalBody.Init();
        }

        //TODO Сделать параметр от типа T. Чтобы нельзя было скормить префаб, неприводимый к Т.
        // Это же позволит избегать ошибок, возникающих при копировании этих однообразных методов.
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

        private void ShowQuitComfirmationModalInner(string caption, bool closeGame)
        {
            var modalBody = CreateWindowHandler<QuitModalBody>(QuitModalPrefab.gameObject);
            modalBody.Init(caption, closeGame);
        }
    }
}
