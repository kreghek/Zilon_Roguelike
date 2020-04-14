namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Базовый класс для задач актёра, которые длятся один ход (открытие сундука, перекладывание вещей).
    /// </summary>
    public abstract class OneTurnActorTaskBase : ActorTaskBase
    {
        protected OneTurnActorTaskBase(IActor actor) : base(actor)
        {

        }

        public override void Execute()
        {
            ExecuteTask();
            IsComplete = true;
        }

        protected abstract void ExecuteTask();
    }
}
