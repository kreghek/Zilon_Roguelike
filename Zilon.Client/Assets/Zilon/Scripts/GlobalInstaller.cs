using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;

public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    public SchemeLocator SchemeLocator;

    public override void InstallBindings()
    {
        Container.Bind<IDice>().FromInstance(new Dice()).AsSingle(); // инстанцируем явно из-за 2-х конструкторов.
        Container.Bind<IDecisionSource>().To<DecisionSource>().AsSingle();
        Container.Bind<ISectorGeneratorRandomSource>().To<SectorGeneratorRandomSource>().AsSingle();
        Container.Bind<ISchemeService>().To<SchemeService>().AsSingle();
        Container.Bind<ISchemeServiceHandlerFactory>().To<SchemeServiceHandlerFactory>().AsSingle();
        Container.Bind<IPropFactory>().To<PropFactory>().AsSingle();
        Container.Bind<IDropResolver>().To<DropResolver>().AsSingle();
        Container.Bind<IDropResolverRandomSource>().To<DropResolverRandomSource>().AsSingle();
        Container.Bind<IPerkResolver>().To<PerkResolver>().AsSingle();


        Container.Bind<HumanPlayer>().AsSingle();
        Container.Bind<IBotPlayer>().To<BotPlayer>().AsSingle();
        Container.Bind<IHumanPersonManager>().To<HumanPersonManager>().AsSingle();


        Container.Bind<ISchemeLocator>().FromInstance(SchemeLocator).AsSingle();
    }
}