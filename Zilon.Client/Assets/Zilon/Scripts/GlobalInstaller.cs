using Assets.Zilon.Scripts.Commands;
using Assets.Zilon.Scripts.DependencyInjection;
using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.Commands;
using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.ScoreResultGenerating;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.World;

public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    public SchemeLocator SchemeLocator;

    public override void InstallBindings()
    {
        Container.Bind<UiSettingService>().AsSingle();

        RegisterDices();

        Container.Bind<IPlayer>().To<HumanPlayer>().AsSingle();

        Container.Bind<IDecisionSource>().To<DecisionSource>().AsSingle();
        Container.Bind<ISchemeService>().To<SchemeService>().AsSingle();
        Container.Bind<ISchemeServiceHandlerFactory>().To<SchemeServiceHandlerFactory>().AsSingle();
        Container.Bind<IPropFactory>().To<PropFactory>().AsSingle();
        Container.RegisterPersonFactory();
        Container.Bind<IDropResolver>().To<DropResolver>().AsSingle();
        Container.Bind<IDropResolverRandomSource>().To<DropResolverRandomSource>().AsSingle();
        Container.Bind<ISurvivalRandomSource>().To<SurvivalRandomSource>().AsSingle();
        Container.Bind<IPerkResolver>().To<PerkResolver>().AsSingle();
        Container.Bind<IScoreManager>().To<ScoreManager>().AsSingle();
        Container.Bind<IPlayerEventLogService>().To<PlayerEventLogService>().AsSingle();
        Container.Bind<DeathReasonService>().AsSingle();
        Container.Bind<ProgressStorageService>().AsSingle();
        Container.Bind<ScoreStorage>().AsSingle();
        Container.Bind<IUserTimeProvider>().To<UserTimeProvider>().AsSingle();

        Container.Bind<IEquipmentDurableService>().To<EquipmentDurableService>().AsSingle();
        Container.Bind<IEquipmentDurableServiceRandomSource>().To<EquipmentDurableServiceRandomSource>().AsSingle();

        Container.Bind<IDiseaseGenerator>().To<DiseaseGenerator>().AsSingle();

        Container.Bind<IBiomeSchemeRoller>().To<BiomeSchemeRoller>().AsSingle();

        Container.Bind<IPersonInitializer>().To<HumanPersonInitializer>().AsSingle();

        Container.Bind<ISchemeLocator>().FromInstance(SchemeLocator).AsSingle();

        Container.Bind<ICommandBlockerService>().To<CommandBlockerService>().AsSingle();

        Container.Bind<ICommand>().WithId("quit-command").To<QuitCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("quit-title-command").To<QuitTitleCommand>().AsSingle();
    }

    private void RegisterDices()
    {
        Container.Bind<IDice>().WithId("linear").To<LinearDice>().AsSingle();
        Container.Bind<IDice>().WithId("exp").To<ExpDice>().AsSingle();
        Container.Bind<IDice>().WithId("gauss").To<GaussDice>().AsSingle();

        // Во всех случаях, когда не нужна кость с конкретным распределением,
        // используем обычную кость с равномерным распределением.
        Container.Bind<IDice>().FromMethod(context => context.Container.ResolveId<IDice>("linear"));
    }
}