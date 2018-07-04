using Assets.Zilon.Scripts.Models.SectorScene;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.CommonServices.MapGenerators;
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
    }
}