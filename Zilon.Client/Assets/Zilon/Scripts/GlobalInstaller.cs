using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.WorldGeneration;

public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    public SchemeLocator SchemeLocator;

    public override void InstallBindings()
    {
        Container.Bind<IDice>().FromInstance(new Dice()).AsSingle(); // инстанцируем явно из-за 2-х конструкторов.
        Container.Bind<IDecisionSource>().To<DecisionSource>().AsSingle();
        Container.Bind<ISchemeService>().To<SchemeService>().AsSingle();
        Container.Bind<ISchemeServiceHandlerFactory>().To<SchemeServiceHandlerFactory>().AsSingle();
        Container.Bind<IPropFactory>().To<PropFactory>().AsSingle();
        Container.Bind<IDropResolver>().To<DropResolver>().AsSingle();
        Container.Bind<IDropResolverRandomSource>().To<DropResolverRandomSource>().AsSingle();
        Container.Bind<IPerkResolver>().To<PerkResolver>().AsSingle();


        Container.Bind<HumanPlayer>().AsSingle();
        Container.Bind<IBotPlayer>().To<BotPlayer>().AsSingle();
        Container.Bind<IHumanPersonManager>().To<HumanPersonManager>().AsSingle();

        Container.Bind<IWorldGenerator>().To<WorldGenerator>().AsSingle();
        Container.Bind<IGlobeManager>().To<GlobeManager>().AsSingle();


        Container.Bind<ISchemeLocator>().FromInstance(SchemeLocator).AsSingle();

        Container.Bind<ICommandBlockerService>().To<CommandBlockerService>().AsSingle();
    }
}