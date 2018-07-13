using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Models.SectorScene;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.CommonServices.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Generation;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();
        Container.Bind<ISectorManager>().To<SectorManager>().AsSingle();
        Container.Bind<IMapGenerator>().To<GridMapGenerator>().AsSingle();
        Container.Bind<IPlayerState>().To<PlayerState>().AsSingle();
        Container.Bind<IDice>().FromInstance(new Dice()).AsSingle();
        Container.Bind<IDecisionSource>().To<DecisionSource>().AsSingle();
        Container.Bind<ISectorGeneratorRandomSource>().To<SectorGeneratorRandomSource>().AsSingle();
        Container.Bind<ISchemeService>().To<SchemeService>().AsSingle();
        Container.Bind<ISchemeLocator>().FromInstance(FindObjectOfType<SchemeLocator>()).AsSingle();
        Container.Bind<IPropFactory>().To<PropFactory>().AsSingle();

        Container.Bind<ICommand>().WithId("move-command").To<MoveCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("attack-command").To<AttackCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("open-container-command").To<OpenContainerCommand>().AsSingle();
    }
}