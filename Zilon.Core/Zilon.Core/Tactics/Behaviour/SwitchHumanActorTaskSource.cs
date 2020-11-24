using System;
using System.Threading.Tasks;

namespace Zilon.Core.Tactics.Behaviour
{
    public enum ActorTaskSourceControl
    {
        Undefined = 0,
        Human = 1,
        Bot = 2
    }

    public class SwitchHumanActorTaskSource<TContext> : IHumanActorTaskSource<TContext> where TContext : ISectorTaskSourceContext
    {
        private readonly IHumanActorTaskSource<TContext> _humanActorTaskSource;
        private readonly IActorTaskSource<TContext> _botActorTaskContext;

        private ActorTaskSourceControl _taskSourceControl;

        public SwitchHumanActorTaskSource(IHumanActorTaskSource<TContext> humanActorTaskSource, IActorTaskSource<TContext> botActorTaskContext)
        {
            _humanActorTaskSource = humanActorTaskSource ?? throw new ArgumentNullException(nameof(humanActorTaskSource));
            _botActorTaskContext = botActorTaskContext ?? throw new ArgumentNullException(nameof(botActorTaskContext));

            _taskSourceControl = ActorTaskSourceControl.Human;
        }

        public void CancelTask(IActorTask cancelledActorTask)
        {
            switch (_taskSourceControl)
            {
                case ActorTaskSourceControl.Human:
                    _humanActorTaskSource.CancelTask(cancelledActorTask);
                    break;

                case ActorTaskSourceControl.Bot:
                    _botActorTaskContext.CancelTask(cancelledActorTask);
                    break;

                case ActorTaskSourceControl.Undefined:
                    //TODO Add common TaskSourceException
                    throw new InvalidOperationException("Task source is undefined.");
                default:
                    throw new InvalidOperationException($"Task source {_taskSourceControl} is unknown.");
            }
        }

        public bool CanIntent()
        {
            throw new NotImplementedException();
        }

        public Task<IActorTask> GetActorTaskAsync(IActor actor, TContext context)
        {
            throw new NotImplementedException();
        }

        public void Intent(IIntention intention, IActor activeActor)
        {
            throw new NotImplementedException();
        }

        public Task IntentAsync(IIntention intention, IActor activeActor)
        {
            throw new NotImplementedException();
        }

        public void ProcessTaskComplete(IActorTask actorTask)
        {
            throw new NotImplementedException();
        }

        public void ProcessTaskExecuted(IActorTask actorTask)
        {
            throw new NotImplementedException();
        }
    }
}