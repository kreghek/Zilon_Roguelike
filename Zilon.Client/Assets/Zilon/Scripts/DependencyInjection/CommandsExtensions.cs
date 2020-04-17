using Assets.Zilon.Scripts.Commands;
using Assets.Zilon.Scripts.Services;
using Zenject;

using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class CommandsExtensions
    {
        /// <summary>
        /// Комманды актёра.
        /// </summary>
        public static void RegisterActorCommands(this DiContainer diContainer)
        {
            diContainer.Bind<ICommand>().WithId("move-command").To<MoveCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("attack-command").To<AttackCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("open-container-command").To<OpenContainerCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("next-turn-command").To<NextTurnCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("use-self-command").To<UseSelfCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("sector-transition-move-command").To<SectorTransitionMoveCommand>().AsSingle();
        }

        /// <summary>
        /// Клиентские команды для Ui.
        /// </summary>
        public static void RegisterUiCommands(this DiContainer diContainer)
        {
            diContainer.Bind<ICommand>().WithId("show-container-modal-command").To<ShowContainerModalCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("show-inventory-command").To<ShowInventoryModalCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("show-perks-command").To<ShowPerksModalCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("show-trader-modal-command").To<ShowTraderModalCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("show-dialog-modal-command").To<ShowDialogModalCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("quit-request-command").To<QuitRequestCommand>().AsSingle();
            diContainer.Bind<ICommand>().WithId("quit-request-title-command").To<QuitTitleRequestCommand>().AsSingle();
        }

        public static void RegisterSpecialCommands(this DiContainer diContainer)
        {
            diContainer.Bind<ICommand>().WithId("equip-command").To<EquipCommand>().AsTransient();
            diContainer.Bind<ICommand>().WithId("prop-transfer-command").To<PropTransferCommand>().AsTransient();

            diContainer.Bind<SpecialCommandManager>().AsSingle();
        }
    }
}
