﻿using Zilon.Core.Tactics;

namespace Zilon.Core.Client.Windows
{
    /// <summary>
    /// Менеджер модальных окон.
    /// Реализация на клиенте.
    /// </summary>
    public interface ISectorModalManager : ICommonModalManager
    {
        void ShowContainerModal(PropTransferMachine transferMachine);

        void ShowInstructionModal();

        void ShowInventoryModal(IActor actor);

        void ShowPerksModal(IActor actor);
    }
}