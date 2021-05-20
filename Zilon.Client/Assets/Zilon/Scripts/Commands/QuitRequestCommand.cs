﻿using Zenject;

using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitRequestCommand : ICommand
    {
        [Inject] ISectorModalManager _sectorModalManager;

        public CanExecuteCheckResult CanExecute()
        {
            return CanExecuteCheckResult.CreateSuccessful();
        }

        public void Execute()
        {
            _sectorModalManager.ShowQuitComfirmationModal();
        }
    }
}
